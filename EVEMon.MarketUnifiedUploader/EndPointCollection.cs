using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.MarketUnifiedUploader
{
    public sealed class EndPointCollection : ReadOnlyCollection<EndPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointCollection"/> class.
        /// </summary>
        internal EndPointCollection()
            : base(new List<EndPoint>())
        {
        }

        /// <summary>
        /// Initializes the endpoints.
        /// </summary>
        internal void InitializeEndPoints()
        {
            // Online EndPoints
            IEnumerable<EndPoint> endpointsOnline = GetOnlineEndPoints();

            // Settings EndPoints
            IEnumerable<EndPoint> endpointsSettings = GetSettingsEndPoints();

            // Merge online and user configuration
            foreach (EndPoint onlineEndPoint in endpointsOnline)
            {
                // Find the endpoint in settings that match the online one
                // Any endpoints not included in online list will be discarded
                // thus assuring that only approved endpoints will be used
                EndPoint settingsEndpoint = endpointsSettings.FirstOrDefault(
                    endpointSettings => endpointSettings.Name == onlineEndPoint.Name);

                // Apply user settings
                if (settingsEndpoint != null)
                    onlineEndPoint.Enabled = settingsEndpoint.Enabled;
            }

            // Import the merged endpoints
            Import(endpointsOnline);

            // Update the settings
            UpdateEndPointSettings();

            // Notify the subscribers
            Uploader.OnEndPointsUpdated();
        }

        /// <summary>
        /// Gets the online endpoints.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<EndPoint> GetOnlineEndPoints()
        {
            List<EndPoint> endPoints = new List<EndPoint>();

            Dictionary<string, object> jsonObj = GetEndPointsOnline();

            if (jsonObj != null && jsonObj.Any())
            {
                ArrayList endPointsList = jsonObj["endpoints"] as ArrayList;
                if (endPointsList == null)
                    return endPoints;

                endPoints.AddRange(
                    endPointsList.OfType<Dictionary<string, object>>()
                        .Select(endPoint =>
                                    {
                                        EndPoint endpoint = new EndPoint
                                                                {
                                                                    Name = endPoint["name"].ToString(),
                                                                    Enabled = Convert.ToBoolean(endPoint["enabled"].ToString())
                                                                };

                                        if (endPoint.Keys.Contains("url"))
                                            endpoint.URL = new Uri(endPoint["url"].ToString());
                                        if (endPoint.Keys.Contains("key"))
                                            endpoint.UploadKey = endPoint["key"].ToString();
                                        if (endPoint.Keys.Contains("gzipSupport"))
                                            endpoint.GzipSupport = Convert.ToBoolean(endPoint["gzipSupport"].ToString());

                                        return endpoint;
                                    }));
            }
            return endPoints;
        }

        /// <summary>
        /// Gets the settings endpoints.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<EndPoint> GetSettingsEndPoints()
        {
            return Settings.MarketUnifiedUploader.EndPoints.Select(
                endPoint => new EndPoint { Name = endPoint.Name, Enabled = endPoint.Enabled });
        }

        /// <summary>
        /// Gets the endpoints online.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, object> GetEndPointsOnline()
        {
            string responce;
            try
            {
                Uri url = new Uri(NetworkConstants.UploaderEndPoints);
                responce = EveMonClient.HttpWebService.DownloadString(url);
            }
            catch (HttpWebServiceException ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionHandler.LogException(ex, true);
                return new Dictionary<string, object>();
            }

            //responce = "{'endpoints':[" +
            //           "{'name':'Default','url':'http://127.0.0.1/','key':'0','gzipSupport':'false','enabled':'false'}," +
            //           "{'name':'EVE Market Data Relay','url':'http://upload.eve-emdr.com/upload/','key':'0','gzipSupport':'true','enabled':'false'}," +
            //           "{'name':'EVE Central','url':'http://eve-central.com/datainput.py/inputdata/','key':'0','gzipSupport':'false','enabled':'true'}," +
            //           "{'name':'EVE Addicts','url':'http://upload.addicts.nl/upload/','key':'fd6e2d2d824da46bc229013e3a5c804a','gzipSupport':'true','enabled':'false'}" +
            //           "]}";

            return Util.DeserializeJSONToObject(responce);
        }

        /// <summary>
        /// Imports the specified endpoints.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        private void Import(IEnumerable<EndPoint> endpoints)
        {
            Items.Clear();
            foreach (EndPoint endpoint in endpoints)
            {
                Items.Add(endpoint);
            }
        }

        /// <summary>
        /// Updates the endpoint settings.
        /// </summary>
        public static void UpdateEndPointSettings()
        {
            Settings.MarketUnifiedUploader.EndPoints.Clear();
            Settings.MarketUnifiedUploader.EndPoints.AddRange(
                Uploader.EndPoints.Select(
                    endPoint => new SerializableEndPoint { Name = endPoint.Name, Enabled = endPoint.Enabled }));
        }
    }
}
