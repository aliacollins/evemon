using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Net;

namespace EVEMon.MarketUnifiedUploader
{
    internal sealed class EndPointCollection : ReadOnlyCollection<EndPoint>
    {
        private const string EndPointFilename = "endpoints.json";
        private readonly string m_settingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), EndPointFilename);
        private bool m_init;

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
            IEnumerable<EndPoint> endpointsOnline = GetEndPoints(GetOnlineEndPoints());

            // Settings EndPoints
            IEnumerable<EndPoint> endpointsSettings = GetEndPoints(GetSettingsEndPoints());

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

            Uploader.OnEndPointsUpdated();
        }

        /// <summary>
        /// Gets the endpoints.
        /// </summary>
        /// <param name="jsonObj">The json obj.</param>
        /// <returns></returns>
        private static IEnumerable<EndPoint> GetEndPoints(IDictionary<string, object> jsonObj)
        {
            List<EndPoint> endPoints = new List<EndPoint>();

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
        private Dictionary<string, object> GetSettingsEndPoints()
        {
            if (!File.Exists(m_settingsFilePath))
                return null;

            try
            {
                string fileContent = File.ReadAllText(m_settingsFilePath);
                return Util.DeserializeJSONToObject(fileContent);
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the online endpoints.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, object> GetOnlineEndPoints()
        {
            string url = NetworkConstants.UploaderEndPoints + EndPointFilename;
            string responce;
            try
            {
                responce = EveMonClient.HttpWebService.DownloadString(new Uri(url));
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

            m_init = true;

            // Save endpoint to file
            SaveEndPoints();
        }

        /// <summary>
        /// Saves the endpoints.
        /// </summary>
        internal void SaveEndPoints()
        {
            // Do not save before endpoints initilization
            if (!m_init)
                return;

            ArrayList endPoints = new ArrayList();
            endPoints.AddRange(Items.Select(item => new Dictionary<string, object>
                                                        {
                                                            { "name", item.Name },
                                                            { "enabled", item.Enabled.ToString() }
                                                        }).ToArray());

            Dictionary<string, object> jsonObj = new Dictionary<string, object> { { "endpoints", endPoints } };
            string json = Util.SerializeObjectToJSON(jsonObj);

            try
            {
                File.WriteAllText(m_settingsFilePath, json);
            }
            catch (IOException ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine(message);
            }
            catch (UnauthorizedAccessException ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine(message);
            }
        }
    }
}
