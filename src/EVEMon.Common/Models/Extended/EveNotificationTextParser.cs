using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Net;
using EVEMon.Common.Service;
using YamlDotNet.RepresentationModel;

namespace EVEMon.Common.Models.Extended
{
    public abstract class EveNotificationTextParser
    {
        public static event EventHandler<EventArgs> NotificationTextParserUpdated;
        
        private static EveNotificationTextParser s_parser;
        private static DateTime s_cachedUntil;
        private static bool s_queryPending;

        /// <summary>
        /// Parses the notification text.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="parsedDict">The parsed dictionary.</param>
        public abstract void Parse(EveNotification notification, KeyValuePair<YamlNode, YamlNode> pair,
            IDictionary<string, string> parsedDict);

        /// <summary>
        /// Gets the parser.
        /// </summary>
        /// <returns></returns>
        internal static EveNotificationTextParser GetParser()
        {
            if (s_parser != null && s_cachedUntil > DateTime.UtcNow)
                return s_parser;

            //if (!EveMonClient.IsDebugBuild)
                Task.Run(GetExternalParser);
            
            return new InternalEveNotificationTextParser();
        }

        /// <summary>
        /// Gets the external parser.
        /// </summary>
        private static async Task GetExternalParser()
        {
            if (s_queryPending)
                return;

            EveMonClient.Trace("begin");

            Uri url = new Uri($"{NetworkConstants.BitBucketWikiBase}{NetworkConstants.ExternalEveNotificationTextParser}");

            s_queryPending = true;

            OnDownloaded(await HttpWebClientService.DownloadStringAsync(url));

            // Reset query pending flag
            s_queryPending = false;
        }

        /// <summary>
        /// Processes the queried notification text parser.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnDownloaded(DownloadAsyncResult<String> result)
        {
            if (result.Error != null)
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace(result.Error.Message);
                return;
            }
            
            string[] referenceAssemblies =
            {
                typeof(Enumerable).Assembly.Location,
                typeof(YamlNode).Assembly.Location,
            };

            s_parser = CodeCompiler.GenerateAssembly<EveNotificationTextParser>(referenceAssemblies, result.Result);

            s_cachedUntil = DateTime.UtcNow.AddHours(12);

            EveMonClient.Trace("done");

            // Notify the subscribers
            NotificationTextParserUpdated?.Invoke(null, EventArgs.Empty);
        }
    }
}