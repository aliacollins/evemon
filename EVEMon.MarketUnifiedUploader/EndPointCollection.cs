using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using EVEMon.Common;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Threading;

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
            Dispatcher.Invoke(Uploader.OnEndPointsUpdated);
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
            string response;
            try
            {
                Uri url = new Uri(NetworkConstants.UploaderEndPoints);
                response = EveMonClient.HttpWebService.DownloadString(url);
            }
            catch (HttpWebServiceException ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionHandler.LogException(ex, true);
                return new Dictionary<string, object>();
            }

            return Util.DeserializeJSONToObject(response);
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

        /// <summary>
        /// Gets a string representation of this collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder endpoints = new StringBuilder();
            foreach (EndPoint endPoint in Items)
            {
                endpoints.Append(endPoint.Name);
                if (endPoint != Items.Last())
                    endpoints.Append(", ");
            }
            return endpoints.ToString();
        }
    }
}
