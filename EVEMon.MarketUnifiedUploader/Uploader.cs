using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using EVEMon.Common;
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

        private static readonly EndPointCollection s_endPoints = new EndPointCollection();

        private static UploaderStatus s_status;
        private static string s_progressText;

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the Uploader is running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the Uploader is running; otherwise, <c>false</c>.
        /// </value>
        public static bool IsRunning { get; private set; }

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
        public static IEnumerable<EndPoint> EndPoints
        {
            get { return s_endPoints; }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Starts the uploader.
        /// </summary>
        public static void Start()
        {
            // Already started
            if (IsRunning)
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
            IsRunning = false;
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
            EveMonClient.Trace("MarketUnifiedUploader.OnEndPointsUpdated - {0}", s_endPoints.ToString());
            if (EndPointsUpdated != null)
                EndPointsUpdated(null, EventArgs.Empty);
        }

        #endregion


        #region Uploader methods

        /// <summary>
        /// Initializes the uploader.
        /// </summary>
        private static void Initialize()
        {
            IsRunning = true;
            Status = UploaderStatus.Initializing;
            s_endPoints.InitializeEndPoints();

            // If there are no available endpoints disable the uploader
            if (!s_endPoints.Any())
            {
                Stop();
                return;
            }

            Parser.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");
            Status = UploaderStatus.Idle;
            Upload();
        }

        /// <summary>
        /// Gets the data, processes them and uploads them to the specified endpoints.
        /// </summary>
        private static void Upload()
        {
            DateTime nextRun = GetNextRun(TimeSpan.FromSeconds(30));

            try
            {
                while (IsRunning)
                {
                    // Is it time to scan?
                    if (nextRun >= DateTime.UtcNow)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        if (!NetworkMonitor.IsNetworkAvailable)
                        {
                            Status = UploaderStatus.Disabled;
                            nextRun = GetNextRun(TimeSpan.FromSeconds(1));
                            Console.WriteLine("Network Unavailable. Next run at: {0}", nextRun);
                            continue;
                        }
                        continue;
                    }

                    // Are there enabled endpoints?
                    if (s_endPoints.All(endPoint => !endPoint.Enabled || endPoint.NextUploadTimeUtc >= DateTime.UtcNow))
                    {
                        nextRun = GetNextRun(TimeSpan.FromMinutes(1));
                        Console.WriteLine("Disabled Endpoints. Next run at: {0}", nextRun);
                        continue;
                    }

                    // Get files from EVE cache and upload the data to the selected endpoints
                    foreach (FileInfo cachedfile in Parser.GetMachoNetCachedFiles())
                    {
                        // Parse the cached file
                        KeyValuePair<object, object> result = ParseCacheFile(cachedfile);

                        // Skip if for some reason the result is null
                        if (result.Key == null || result.Value == null)
                            continue;

                        // Create the JSON object
                        Dictionary<string, object> jsonObj = UnifiedFormat.GetJSONObject(result);

                        // Skip if for some reason the JSON object is null or empty
                        if (jsonObj == null || !jsonObj.Any())
                            continue;

                        // Uploads to the selected endpoints
                        UploadToEndPoints(cachedfile, jsonObj);
                    }

                    Status = UploaderStatus.Idle;
                    nextRun = GetNextRun(TimeSpan.FromMinutes(1));
                    Console.WriteLine("Next run at: {0}", nextRun);
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
                Settings.MarketUnifiedUploader.Enabled = false;
                Stop();
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
            string data = Util.SerializeObjectToJSON(jsonObj);

            // Upload to the selected endpoints
            foreach (EndPoint endPoint in endPoints.Where(endPoint => endPoint.Enabled &&
                                                                      endPoint.NextUploadTimeUtc < DateTime.UtcNow))
            {
                string postdata = GetPostDataFormat(endPoint.Method, data);

                Status = UploaderStatus.Uploading;
                ProgressText = GetProcessText(jsonObj, endPoint);
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
                response = EveMonClient.HttpWebService.DownloadString(endPoint.Url, endPoint.Method, postdata,
                                                                      endPoint.Compression);
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
        private static string GetProcessText(IDictionary<string, object> jsonObj, EndPoint endPoint)
        {
            // Gather info to use in progress message
            string resultType = jsonObj["resultType"].ToString();
            string typeID = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                .Select(row => row["typeID"]).First().ToString();
            string regionID = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                .Select(row => row["regionID"]).First().ToString();
            int rowsCount = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                .Select(rowset => rowset["rows"]).OfType<ArrayList>().First().Count;

            return String.Format("{5}: Uploading to {0}, {1}: {2}, typeID: {3}, region: {4}{6}",
                                 endPoint.Name, resultType, rowsCount, typeID, regionID,
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

            // Postpone next upload try for 10 minutes accumulatively; up to 1 day if uploading fails repeatedly
            if (response != "1")
            {
                if (endPoint.UploadInterval < TimeSpan.FromDays(1))
                    endPoint.UploadInterval = endPoint.UploadInterval.Add(TimeSpan.FromMinutes(10));

                endPoint.NextUploadTimeUtc = DateTime.UtcNow.Add(endPoint.UploadInterval);

                ProgressText = String.Format("{3}: {0}{4}Next upload try to {1} at: {2}{4}", response,
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

                ProgressText = String.Format("{1}: Uploaded to {0} succesfully.{2}",
                                             endPoint.Name, DateTime.Now.ToUniversalDateTimeString(),
                                             Environment.NewLine);

                Console.Write(s_progressText);

                // Delete the cached file
                try
                {
                    Console.WriteLine("Deleting cache file.");
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
                    return String.Format("data={0}", HttpUtility.UrlEncode(data));
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
