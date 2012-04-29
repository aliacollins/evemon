using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using EVEMon.Common.Net;
using EVEMon.MarketUnifiedUploader.EveCacheParser;

namespace EVEMon.MarketUnifiedUploader
{
    public enum UploaderStatus
    {
        Disabled = 0,
        Initializing = 1,
        Idle = 2,
        Uploading = 3,
    }

    public static class Uploader
    {
        public static event EventHandler ProgressTextChanged;
        public static event EventHandler StatusChanged;
        public static event EventHandler EndPointsUpdated;

        private static readonly EndPointCollection s_endPoints = new EndPointCollection();
        private static readonly HttpWebService s_webService = new HttpWebService();

        private static UploaderStatus s_status;
        private static bool s_run;
        private static string s_progressText;

        public static string ProgressText
        {
            get { return s_progressText; }
            private set
            {
                s_progressText = value;
                OnProgressTextChanged();
            }
        }

        public static UploaderStatus Status
        {
            get { return s_status; }
            private set
            {
                s_status = value;
                OnStatusChanged();
            }
        }

        public static IEnumerable<EndPoint> EndPoints
        {
            get { return s_endPoints; }
        }

        internal static HttpWebService WebService
        {
            get { return s_webService; }
        }

        public static void SaveEndPoints()
        {
            s_endPoints.SaveEndPoints();
        }

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

        public static void Stop()
        {
            Status = UploaderStatus.Disabled;
            s_run = false;
        }

        private static void OnStatusChanged()
        {
            if (StatusChanged != null)
                StatusChanged(null, EventArgs.Empty);
        }

        private static void OnProgressTextChanged()
        {
            if (ProgressTextChanged != null)
                ProgressTextChanged(null, EventArgs.Empty);
        }

        public static void OnEndPointsUpdated()
        {
            if (EndPointsUpdated != null)
                EndPointsUpdated(null, EventArgs.Empty);
        }

        private static DateTime NextRun(bool init = false)
        {
            return init ? DateTime.UtcNow.AddSeconds(5) : DateTime.UtcNow.AddMinutes(1);
        }

        private static void Upload()
        {
            DateTime nextRun = NextRun(true);
            Parser.SetIncludeMethodsFilter("GetOrders", "GetOldPriceHistory", "GetNewPriceHistory");

            while (s_run)
            {
                // Is it time to scan?
                if (nextRun >= DateTime.UtcNow)
                    continue;

                if (!NetworkMonitor.IsNetworkAvailable)
                {
                    Status = UploaderStatus.Disabled;
                    nextRun = NextRun();
                    Console.WriteLine("Network Unavailable. Next run at: {0}", nextRun);
                    continue;
                }

                // Are there enabled endpoints?
                if (s_endPoints.All(endPoint => !endPoint.Enabled || endPoint.NextUploadTimeUtc >= DateTime.UtcNow))
                {
                    nextRun = NextRun();
                    Console.WriteLine("Disabled Endpoints. Next run at: {0}", nextRun);
                    continue;
                }

                // Get files from EVE cache and upload the data to the selected endpoints
                foreach (FileInfo cachedfile in Parser.GetMachoNetCachedFiles())
                {
                    // Parse the cached file
                    KeyValuePair<object, object> result;
                    try
                    {
                        result = Parser.Parse(cachedfile);
                    }
                    catch (ParserException ex)
                    {
                        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        Console.WriteLine(message);
                        //ExceptionHandler.LogException(ex, false);
                        continue;
                    }

                    // Create the JSON object
                    Dictionary<string, object> jsonObj = UnifiedFormat.GetJSONObject(result);

                    // Skip if for some reason the JSON object is null or empty
                    if (jsonObj == null || !jsonObj.Any())
                        continue;

                    // Gather info to use in progress message
                    string resultType = jsonObj["resultType"].ToString();
                    string typeID = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                        .Select(row => row["typeID"]).First().ToString();
                    string regionID = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                        .Select(row => row["regionID"]).First().ToString();
                    int rowsCount = ((ArrayList)jsonObj["rowsets"]).OfType<Dictionary<string, object>>()
                        .Select(rowset => rowset["rows"]).OfType<ArrayList>().First().Count;

                    // Get the endpoints the message was generated for;
                    // a check to where we can upload to has been made while generating the message
                    IEnumerable<EndPoint> endPoints =
                        ((ArrayList)jsonObj["uploadKeys"]).OfType<Dictionary<string, object>>().Select(
                            key => key["name"].ToString()).SelectMany(
                                name => s_endPoints, (name, endPoint) => new { name, endPoint }).Where(
                                    endpoint => endpoint.endPoint.Name == endpoint.name).Select(
                                        endpoint => endpoint.endPoint);

                    // Serialize the JSON object to string
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string postdata = serializer.Serialize(jsonObj);

                    // Upload to the selected endpoints
                    foreach (EndPoint endPoint in endPoints)
                    {
                        Status = UploaderStatus.Uploading;

                        string responce;
                        try
                        {
                            ProgressText = String.Format("{5}: Uploading to {0}, {1}: {2}, typeID: {3}, region: {4}{6}",
                                                         endPoint.Name, resultType, rowsCount, typeID, regionID,
                                                         DateTime.Now.ToUniversalDateTimeString(), Environment.NewLine);
                            Console.Write(s_progressText);

                            responce = s_webService.DownloadString(endPoint.URL, postdata, endPoint.GzipSupport);
                        }
                        catch (HttpWebServiceException ex)
                        {
                            responce = ex.Message;
                        }

                        // Postpone next upload try for 10 minutes accumulatively; up to 1 day if uploading fails repeatedly
                        if (responce != "1")
                        {
                            if (endPoint.UploadInterval < TimeSpan.FromDays(1))
                                endPoint.UploadInterval = endPoint.UploadInterval.Add(TimeSpan.FromMinutes(10));

                            endPoint.NextUploadTimeUtc = DateTime.UtcNow.Add(endPoint.UploadInterval);

                            ProgressText = String.Format("{3}: {0}{4}Next upload try to {1} at: {2}{4}", responce,
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
                                //ExceptionHandler.LogException(ex, false);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Console.WriteLine(ex.Message);
                                //ExceptionHandler.LogException(ex, false);
                            }
                        }
                    }
                }

                Status = UploaderStatus.Idle;

                nextRun = NextRun();
                Console.WriteLine("Next run at: {0}", nextRun);
            }
        }
    }
}
