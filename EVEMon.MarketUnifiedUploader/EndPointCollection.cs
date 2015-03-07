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
            List<EndPoint> onlineEndPoints = GetOnlineEndPoints();

            // Settings EndPoints
            List<EndPoint> settingsEndPoints = GetSettingsEndPoints();

            // Merge online and user configuration
            foreach (EndPoint onlineEndPoint in onlineEndPoints)
            {
                // Find the endpoint in settings that match the online one
                // Any endpoints not included in online list will be discarded
                // thus assuring that only approved endpoints will be used
                EndPoint settingsEndpoint = settingsEndPoints.FirstOrDefault(
                    settingEndpoint => settingEndpoint.Name == onlineEndPoint.Name);

                // Apply user settings
                if (settingsEndpoint != null)
                    onlineEndPoint.Enabled = settingsEndpoint.Enabled;
            }

            // If a valid localhost endpoint is specified insert it on top of the list
            List<SerializableLocalhostEndPoint> localhosts =
                Settings.MarketUnifiedUploader.EndPoints.OfType<SerializableLocalhostEndPoint>().Where(
                    endPoint => endPoint.Url != null && (endPoint.Url.Host == "localhost" || endPoint.Url.Host == "127.0.0.1"))
                        .Reverse().ToList();

            foreach (SerializableLocalhostEndPoint localhost in localhosts)
            {
                onlineEndPoints.Insert(0, new EndPoint(localhost));
            }

            // Import the merged endpoints
            Import(onlineEndPoints);

            // Update the settings
            UpdateSettings();

            // Notify the subscribers
            Dispatcher.Invoke(Uploader.OnEndPointsUpdated);
        }

        /// <summary>
        /// Gets the online endpoints.
        /// </summary>
        /// <returns></returns>
        private static List<EndPoint> GetOnlineEndPoints()
        {
            List<EndPoint> endPoints = new List<EndPoint>();

            Dictionary<string, object> jsonObj = GetEndPointsOnline();

            if (jsonObj == null || !jsonObj.Any())
                return endPoints;

            ArrayList endPointsList = jsonObj["endpoints"] as ArrayList;
            if (endPointsList == null)
                return endPoints;

            endPoints.AddRange(endPointsList.OfType<Dictionary<string, object>>().Select(endPoint => new EndPoint(endPoint)));
            return endPoints;
        }

        /// <summary>
        /// Gets the settings endpoints.
        /// </summary>
        /// <returns></returns>
        private static List<EndPoint> GetSettingsEndPoints()
        {
            return Settings.MarketUnifiedUploader.EndPoints.Where(
                endPoint => !(endPoint is SerializableLocalhostEndPoint)).Select(
                    endPoint => new EndPoint { Name = endPoint.Name, Enabled = endPoint.Enabled }).ToList();
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
                Uri url =
                    new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BitBucketWikiBase,
                        NetworkConstants.UploaderEndPoints));
                response = HttpWebService.DownloadString(url);
            }
            catch (HttpWebServiceException ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionHandler.LogException(ex, true);
                return new Dictionary<string, object>();
            }

            return Util.DeserializeJsonToObject(response);
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
        /// Updates the localhosts.
        /// </summary>
        /// <param name="localhostEndPoints">The localhost endpoints.</param>
        public void UpdateLocalhosts(IEnumerable<SerializableLocalhostEndPoint> localhostEndPoints)
        {
            RemoveLocalhosts();
            foreach (EndPoint endPoint in localhostEndPoints.Reverse().Select(localhostEndPoint => new EndPoint(localhostEndPoint)))
            {
                Items.Insert(0, endPoint);
            }

            Uploader.OnEndPointsUpdated();
        }

        /// <summary>
        /// Removes the localhosts.
        /// </summary>
        private void RemoveLocalhosts()
        {
            List<EndPoint> endpointsToRemove =
                Items.Where(endPoint => endPoint.Url.Host == "localhost" || endPoint.Url.Host == "127.0.0.1").ToList();
            foreach (EndPoint endPoint in endpointsToRemove)
            {
                Items.Remove(endPoint);
            }
        }

        /// <summary>
        /// Updates the endpoint settings.
        /// </summary>
        public void UpdateSettings()
        {
            Settings.MarketUnifiedUploader.EndPoints.Clear();

            foreach (EndPoint endPoint in Items)
            {
                if (endPoint.Url.Host == "localhost" || endPoint.Url.Host == "127.0.0.1")
                {
                    Settings.MarketUnifiedUploader.EndPoints.Add(new SerializableLocalhostEndPoint
                                                                     {
                                                                         Enabled = endPoint.Enabled,
                                                                         Name = endPoint.Name,
                                                                         Url = endPoint.Url,
                                                                         UploadKey = endPoint.UploadKey,
                                                                         Method = endPoint.Method,
                                                                         DataCompression = endPoint.DataCompression
                                                                     });
                    continue;
                }

                Settings.MarketUnifiedUploader.EndPoints.Add(new SerializableEndPoint
                                                                 {
                                                                     Name = endPoint.Name,
                                                                     Enabled = endPoint.Enabled
                                                                 });
            }
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
