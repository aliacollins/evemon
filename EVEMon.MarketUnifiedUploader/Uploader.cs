using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Net;
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
        private static bool s_run;
        private static string s_progressText;

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
                OnProgressTextChanged();
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
                s_status = value;
                OnStatusChanged();
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
        /// Saves the endpoints.
        /// </summary>
        public static void SaveEndPoints()
        {
            s_endPoints.SaveEndPoints();
        }

        /// <summary>
        /// Starts the uploader.
        /// </summary>
        public static void Start()
        {
            Status = UploaderStatus.Initializing;

            NetworkMonitor.Initialize();
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                Stop();
                return;
            }

            s_endPoints.InitializeEndPoints();
            if (!s_endPoints.Any())
            {
                Stop();
                return;
            }

            Status = UploaderStatus.Idle;
            s_run = true;
            Upload();
        }

        /// <summary>
        /// Stops the uploader.
        /// </summary>
        public static void Stop()
        {
            Status = UploaderStatus.Disabled;
            s_run = false;
        }

        #endregion


        #region Events

        /// <summary>
        /// Called when the status changed.
        /// </summary>
        private static void OnStatusChanged()
        {
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
        /// Called when the endpoints updated.
        /// </summary>
        public static void OnEndPointsUpdated()
        {
            if (EndPointsUpdated != null)
                EndPointsUpdated(null, EventArgs.Empty);
        }

        #endregion


        #region Uploader methods

        /// <summary>
        /// Gets the next run.
        /// </summary>
        /// <param name="init">if set to <c>true</c> returns the initial time.</param>
        /// <returns></returns>
        private static DateTime GetNextRun(bool init = false)
        {
            return init ? DateTime.UtcNow.AddSeconds(5) : DateTime.UtcNow.AddMinutes(1);
        }

        /// <summary>
        /// Gets the data, processes them and uploads them to the specified endpoints.
        /// </summary>
        private static void Upload()
        {
            DateTime nextRun = GetNextRun(true);
            Parser.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");

            while (s_run)
            {
                // Is it time to scan?
                if (nextRun >= DateTime.UtcNow)
                    continue;

                if (!NetworkMonitor.IsNetworkAvailable)
                {
                    Status = UploaderStatus.Disabled;
                    nextRun = GetNextRun();
                    Console.WriteLine("Network Unavailable. Next run at: {0}", nextRun);
                    continue;
                }

                // Are there enabled endpoints?
                if (s_endPoints.All(endPoint => !endPoint.Enabled || endPoint.NextUploadTimeUtc >= DateTime.UtcNow))
                {
                    nextRun = GetNextRun();
                    Console.WriteLine("Disabled Endpoints. Next run at: {0}", nextRun);
                    continue;
                }

                // Get files from EVE cache and upload the data to the selected endpoints
                foreach (FileInfo cachedfile in Parser.GetMachoNetCachedFiles())
                {
                    // Parse the cached file
                    KeyValuePair<object, object> result = ParseCacheFile(cachedfile);

                    // Create the JSON object
                    Dictionary<string, object> jsonObj = UnifiedFormat.GetJSONObject(result);

                    // Skip if for some reason the JSON object is null or empty
                    if (jsonObj == null || !jsonObj.Any())
                        continue;

                    // Get the endpoints the message was generated for;
                    // a check to where we can upload to has been made while generating the message
                    IEnumerable<EndPoint> endPoints =
                        ((ArrayList)jsonObj["uploadKeys"]).OfType<Dictionary<string, object>>().Select(
                            key => key["name"].ToString()).SelectMany(
                                name => s_endPoints, (name, endPoint) => new { name, endPoint }).Where(
                                    endpoint => endpoint.endPoint.Name == endpoint.name).Select(
                                        endpoint => endpoint.endPoint);

                    // Serialize the JSON object to string
                    string postdata = Util.SerializeObjectToJSON(jsonObj);

                    // Upload to the selected endpoints
                    foreach (EndPoint endPoint in endPoints)
                    {
                        // Upload to endpoint
                        string response = UploadToEndPoint(postdata, endPoint, jsonObj);

                        // On response act accordingly
                        OnUploaded(cachedfile, response, endPoint);
                    }
                }

                Status = UploaderStatus.Idle;

                nextRun = GetNextRun();
                Console.WriteLine("Next run at: {0}", nextRun);
            }
        }

        /// <summary>
        /// Uploads to end point.
        /// </summary>
        /// <param name="postdata">The postdata.</param>
        /// <param name="endPoint">The endpoint.</param>
        /// <param name="jsonObj">The json obj.</param>
        /// <returns></returns>
        private static string UploadToEndPoint(string postdata, EndPoint endPoint, IDictionary<string, object> jsonObj)
        {
            Status = UploaderStatus.Uploading;

            string response;
            try
            {
                ProgressText = GetProcessText(jsonObj, endPoint);

                Console.Write(s_progressText);

                response = EveMonClient.HttpWebService.DownloadString(endPoint.URL, postdata, endPoint.GzipSupport);
            }
            catch (HttpWebServiceException ex)
            {
                response = ex.Message;
                ExceptionHandler.LogException(ex, false);
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
        private static void OnUploaded(FileSystemInfo cachedfile, string response, EndPoint endPoint)
        {
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
                // Inform about a succesful upload and delete the cached file
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
                    ExceptionHandler.LogException(ex, false);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex.Message);
                    ExceptionHandler.LogException(ex, false);
                }
            }
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
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine(message);
                ExceptionHandler.LogException(ex, false);
                return new KeyValuePair<object, object>();
            }
            return result;
        }

        #endregion
    }
}
