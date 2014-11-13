using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using EVEMon.Common;
using EVEMon.Common.Data;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;
using EVEMon.MarketUnifiedUploader.EveCacheParser;

namespace EVEMon.MarketUnifiedUploader
{
    public static class Uploader
    {
        #region Fields

        public static event EventHandler ProgressTextChanged;
        public static event EventHandler StatusChanged;
        public static event EventHandler EndPointsUpdated;

        private static readonly EndPointCollection s_endPointCollection = new EndPointCollection();
        private static List<EndPoint> s_endPoints = new List<EndPoint>();

        private static UploaderStatus s_status;
        private static string s_progressText;
        private static bool s_isRunning;

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the progress text.
        /// </summary>
        public static string ProgressText
        {
            get { return s_progressText; }
            private set
            {
                s_progressText = value;
                Dispatcher.Invoke(OnProgressTextChanged);
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public static UploaderStatus Status
        {
            get { return s_status; }
            private set
            {
                if (s_status == value)
                    return;

                s_status = value;
                Dispatcher.Invoke(OnStatusChanged);
            }
        }

        /// <summary>
        /// Gets the endpoints.
        /// </summary>
        public static EndPointCollection EndPoints
        {
            get { return s_endPointCollection; }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Starts the uploader.
        /// </summary>
        public static void Start()
        {
            // Already started
            if (s_isRunning)
                return;

            // When there is no network connection retry
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                Dispatcher.Schedule(TimeSpan.FromSeconds(1), Start);
                return;
            }

            EveMonClient.Trace("MarketUnifiedUploader.Start()");
            Dispatcher.BackgroundInvoke(Initialize);
        }

        /// <summary>
        /// Stops the uploader.
        /// </summary>
        public static void Stop()
        {
            s_isRunning = false;
            Status = UploaderStatus.Disabled;
            EveMonClient.Trace("MarketUnifiedUploader.Stop()");
        }

        #endregion


        #region Events

        /// <summary>
        /// Called when the status changed.
        /// </summary>
        private static void OnStatusChanged()
        {
            EveMonClient.Trace("MarketUnifiedUploader.OnStatusChanged - {0}", s_status);
            if (StatusChanged != null)
                StatusChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the progress text changed.
        /// </summary>
        private static void OnProgressTextChanged()
        {
            if (ProgressTextChanged != null)
                ProgressTextChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the endpoints have been updated.
        /// </summary>
        public static void OnEndPointsUpdated()
        {
            EveMonClient.Trace("MarketUnifiedUploader.OnEndPointsUpdated - {0}", s_endPointCollection.ToString());
            if (EndPointsUpdated != null)
                EndPointsUpdated(null, EventArgs.Empty);
        }

        #endregion


        #region Uploader Methods

        /// <summary>
        /// Initializes the uploader.
        /// </summary>
        private static void Initialize()
        {
            s_isRunning = true;

            Status = UploaderStatus.Initializing;

            CheckEndPointsSynchronization();

            Status = UploaderStatus.Idle;

            Parser.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");

            Upload();
        }

        /// <summary>
        /// Checks the end points synchronization.
        /// </summary>
        private static void CheckEndPointsSynchronization()
        {
            if (!Settings.MarketUnifiedUploader.Enabled)
                return;

            // Do it now if network available
            if (NetworkMonitor.IsNetworkAvailable)
            {
                EveMonClient.Trace("MarketUnifiedUploader.EndPointsUpdating()");

                s_endPointCollection.InitializeEndPoints();
                s_endPoints = s_endPointCollection.ToList();

                Dispatcher.Schedule(TimeSpan.FromHours(1), CheckEndPointsSynchronization);

                // If there are no available endpoints disable the uploader
                if (!s_endPoints.Any())
                    Stop();

                return;
            }

            // Reschedule later otherwise
            Dispatcher.Schedule(TimeSpan.FromSeconds(1), CheckEndPointsSynchronization);
        }

        /// <summary>
        /// Gets the data, processes them and uploads them to the specified endpoints.
        /// </summary>
        private static void Upload()
        {
            DateTime nextRun = GetNextRun(TimeSpan.FromSeconds(30));

            try
            {
                while (s_isRunning)
                {
                    // Is it time to scan?
                    if (nextRun >= DateTime.UtcNow)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        if (!NetworkMonitor.IsNetworkAvailable)
                        {
                            Status = UploaderStatus.Disabled;
                            nextRun = GetNextRun(TimeSpan.FromSeconds(1));
                            Console.WriteLine(@"Network Unavailable. Next run at: {0}", nextRun);
                        }
                        continue;
                    }

                    // Are there enabled endpoints?
                    if (s_endPoints.All(endPoint => !endPoint.Enabled || endPoint.NextUploadTimeUtc >= DateTime.UtcNow))
                    {
                        nextRun = GetNextRun(TimeSpan.FromMinutes(1));
                        Console.WriteLine(@"Disabled Endpoints. Next run at: {0}", nextRun);
                        continue;
                    }

                    ParseFilesAndUpload();

                    Status = UploaderStatus.Idle;
                    nextRun = GetNextRun(TimeSpan.FromMinutes(1));
                    Console.WriteLine(@"Next run at: {0}", nextRun);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Dispatcher.Invoke(() => ExceptionHandler.LogException(ex, true));

                // Disable the uploader
                ProgressText = String.Format(CultureConstants.DefaultCulture,
                                             "{1}: {0}{2}",
                                             ex.InnerException == null
                                                 ? ex.Message
                                                 : ex.InnerException.Message,
                                             DateTime.Now.ToUniversalDateTimeString(),
                                             Environment.NewLine);

                Stop();
            }
        }

        /// <summary>
        /// Parses the files and upload.
        /// </summary>
        private static void ParseFilesAndUpload()
        {
            // Get the cache files according to eve clients installations
            FileInfo[] cachedFiles = EveMonClient.EveAppDataFoldersExistInDefaultLocation
                                         ? Parser.GetMachoNetCachedFiles()
                                         : Settings.PortableEveInstallations.EVEClients.SelectMany(
                                             eveClient => Parser.GetMachoNetCachedFiles(eveClient.Path)).ToArray();

            // Parse the cached files and upload the data to the selected endpoints
            foreach (FileInfo cachedFile in cachedFiles.Where(file => file.Exists))
            {
                // Delete older than 90 days cached files
                if (cachedFile.LastWriteTimeUtc.AddDays(90) < DateTime.UtcNow)
                {
                    DeleteCachedFile(cachedFile);
                    continue;
                }

                // Parse the cached file
                KeyValuePair<object, object> result = ParseCacheFile(cachedFile);

                // Skip if there is no result
                if (result.Key == null || result.Value == null)
                    continue;

                // Create the JSON object
                Dictionary<string, object> jsonObj = UnifiedFormat.GetJSONObject(result);

                // Skip if for some reason there is no JSON object or it's empty
                if (jsonObj == null || !jsonObj.Any())
                    continue;

                // Uploads to the selected endpoints
                UploadToEndPoints(cachedFile, jsonObj);
            }
        }

        /// <summary>
        /// Gets the next run.
        /// </summary>
        /// <param name="delay">The delay to add.</param>
        /// <returns></returns>
        private static DateTime GetNextRun(TimeSpan delay)
        {
            return DateTime.UtcNow.Add(delay);
        }

        /// <summary>
        /// Parses the cache file.
        /// </summary>
        /// <param name="cachedfile">The cachedfile.</param>
        /// <returns></returns>
        private static KeyValuePair<object, object> ParseCacheFile(FileInfo cachedfile)
        {
            KeyValuePair<object, object> result;
            try
            {
                result = Parser.Parse(cachedfile);
            }
            catch (ParserException ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                Console.WriteLine(message);
                Dispatcher.Invoke(() => ExceptionHandler.LogException(ex, true));
                return new KeyValuePair<object, object>(null, null);
            }
            return result;
        }

        /// <summary>
        /// Uploads to the selected endpoints
        /// </summary>
        /// <param name="cachedfile">The cachedfile.</param>
        /// <param name="jsonObj">The json obj.</param>
        private static void UploadToEndPoints(FileSystemInfo cachedfile, Dictionary<string, object> jsonObj)
        {
            // Get the endpoints the message was generated for;
            // a check to where we can upload to has been made while generating the message
            IEnumerable<EndPoint> endPoints =
                ((ArrayList)jsonObj["uploadKeys"]).OfType<Dictionary<string, object>>().Select(
                    key => key["name"].ToString()).SelectMany(
                        name => s_endPoints, (name, endPoint) => new { name, endPoint }).Where(
                            endpoint => endpoint.endPoint.Name == endpoint.name).Select(
                                endpoint => endpoint.endPoint);

            // Serialize the JSON object to string
            string data = Util.SerializeObjectToJson(jsonObj);

            // Inform about suspended endpoints
            foreach (EndPoint endPoint in s_endPoints.Where(endPoint => endPoint.Enabled &&
                                                                        endPoint.NextUploadTimeUtc != DateTime.MinValue &&
                                                                        endPoint.NextUploadTimeUtc >= DateTime.UtcNow))
            {
                ProgressText = String.Format(CultureConstants.DefaultCulture, "{2}: Uploading to {0} is suspended until {1}{3}",
                                             endPoint.Name, endPoint.NextUploadTimeUtc.ToLocalTime(),
                                             DateTime.Now.ToUniversalDateTimeString(), Environment.NewLine);
            }

            // Upload to the selected endpoints
            foreach (EndPoint endPoint in endPoints)
            {
                // Exclude HistoryOrders for EMDR
                if (endPoint.Name == "EVE Market Data Relay" && ReferenceEquals(jsonObj["resultType"], "history"))
                    continue;

                string postdata = GetPostDataFormat(endPoint.Method, data);

                Status = UploaderStatus.Uploading;
                ProgressText = GetProgressText(jsonObj, endPoint);
                Console.Write(s_progressText);

                // Upload to endpoint
                string response = UploadToEndPoint(postdata, endPoint);

                // On response act accordingly
                OnUploaded(response, cachedfile, endPoint);
            }
        }

        /// <summary>
        /// Uploads to end point.
        /// </summary>
        /// <param name="postdata">The postdata.</param>
        /// <param name="endPoint">The endpoint.</param>
        /// <returns></returns>
        private static string UploadToEndPoint(string postdata, EndPoint endPoint)
        {
            string response;
            try
            {
                response = HttpWebService.DownloadString(endPoint.Url, endPoint.Method, false, postdata,
                                                         endPoint.DataCompression);
            }
            catch (HttpWebServiceException ex)
            {
                response = ex.Message;
                Dispatcher.Invoke(() => ExceptionHandler.LogException(ex, true));
            }
            return response;
        }

        /// <summary>
        /// Gets the process text.
        /// </summary>
        /// <param name="jsonObj">The json obj.</param>
        /// <param name="endPoint">The endpoint.</param>
        /// <returns></returns>
        private static string GetProgressText(IDictionary<string, object> jsonObj, EndPoint endPoint)
        {
            // Gather info to use in progress message
            String resultType = jsonObj["resultType"].ToString();
            String typeID = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                                                           .Select(row => row["typeID"]).First().ToString();
            String regionID = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                                                              .Select(row => row["regionID"]).First().ToString();
            Int32 rowsCount = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                .Select(rowset => rowset["rows"]).OfType<ArrayList>().First().Count;

            // Try parse typeID to an EVE item
            Int32 eveTypeID;
            Item item = Int32.TryParse(typeID, out eveTypeID) ? StaticItems.GetItemByID(eveTypeID) : null;

            // Try parse regionID to an EVE region
            Int32 eveRegionID;
            Region region = Int32.TryParse(regionID, out eveRegionID) ? StaticGeography.GetRegionByID(eveRegionID) : null;
            
            String itemName = item == null ? String.Empty : String.Format(CultureConstants.DefaultCulture, " ({0})", item.Name);
            String regionName = region == null ? String.Empty : String.Format(CultureConstants.DefaultCulture, " ({0})", region.Name);

            return String.Format(CultureConstants.DefaultCulture,
                                 "{7}: Uploading to {0}, {1}: {2}, typeID: {3}{4}, region: {5}{6}{8}",
                                 endPoint.Name, resultType, rowsCount, typeID, itemName, regionID, regionName,
                                 DateTime.Now.ToUniversalDateTimeString(), Environment.NewLine);
        }

        /// <summary>
        /// Called when data have been uploaded.
        /// </summary>
        /// <param name="cachedfile">The cachedfile.</param>
        /// <param name="response">The responce.</param>
        /// <param name="endPoint">The end point.</param>
        private static void OnUploaded(string response, FileSystemInfo cachedfile, EndPoint endPoint)
        {
            // Special cleaning to prevent issues with responses from different platform servers
            response = response.Replace(Environment.NewLine, String.Empty).Trim();

            // Suspend next upload try for 5 minutes accumulatively; up to 15 minutes if uploading fails repeatedly
            if (response != "1")
            {
                // Suspend uploading according to error type
                if (endPoint.UploadInterval < TimeSpan.FromMinutes(15))
                {
                    endPoint.UploadInterval = !response.Contains(WebExceptionStatus.KeepAliveFailure.ToString()) &&
                                              !response.Contains(WebExceptionStatus.ReceiveFailure.ToString())
                                                  ? endPoint.UploadInterval.Add(TimeSpan.FromMinutes(5))
                                                  : endPoint.UploadInterval.Add(TimeSpan.FromMinutes(1));
                }
                else if (endPoint.UploadInterval > TimeSpan.FromMinutes(15))
                {
                    endPoint.UploadInterval = TimeSpan.FromMinutes(15);
                }

                endPoint.NextUploadTimeUtc = DateTime.UtcNow.Add(endPoint.UploadInterval);

                ProgressText = String.Format(CultureConstants.DefaultCulture, "{3}: {0}{4}Next upload try to {1} at: {2}{4}",
                                             response,
                                             endPoint.Name,
                                             endPoint.NextUploadTimeUtc.ToLocalTime(),
                                             DateTime.Now.ToUniversalDateTimeString(),
                                             Environment.NewLine);

                Console.Write(s_progressText);
            }
                // Inform about a successful upload and delete the cached file
            else
            {
                endPoint.UploadInterval = TimeSpan.Zero;
                endPoint.NextUploadTimeUtc = DateTime.UtcNow;

                ProgressText = String.Format(CultureConstants.DefaultCulture, "{1}: Uploaded to {0} successfully.{2}",
                                             endPoint.Name, DateTime.Now.ToUniversalDateTimeString(),
                                             Environment.NewLine);

                Console.Write(s_progressText);

                // Delete the cached file
                DeleteCachedFile(cachedfile);
            }
        }

        /// <summary>
        /// Deletes the cached file.
        /// </summary>
        /// <param name="cachedfile">The cachedfile.</param>
        private static void DeleteCachedFile(FileSystemInfo cachedfile)
        {
            if (!cachedfile.Exists)
                return;

            try
            {
                Console.WriteLine(@"Deleting cache file.");
                cachedfile.Delete();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                Dispatcher.Invoke(() => ExceptionHandler.LogException(ex, false));
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                Dispatcher.Invoke(() => ExceptionHandler.LogException(ex, false));
            }
        }

        /// <summary>
        /// Gets the post data format.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static string GetPostDataFormat(HttpMethod method, string data)
        {
            switch (method)
            {
                case HttpMethod.Postentity:
                case HttpMethod.Put:
                    return data;
                case HttpMethod.Get:
                case HttpMethod.Post:
                    return String.Format(CultureConstants.InvariantCulture, "data={0}", HttpUtility.UrlEncode(data));
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
