using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Net;
using EVEMon.Common.Service;
using EVEMon.Common.Threading;
using YamlDotNet.RepresentationModel;

namespace EVEMon.Common.Models.Extended
{
    public abstract class EveNotificationTextParser
    {
        private static EveNotificationTextParser s_parser;
        private static DateTime s_cachedUntil;
        private static bool s_queryPending;

        private static readonly string[] s_referenceAssemblies =
        {
            typeof(Enumerable).Assembly.Location,
            typeof(YamlNode).Assembly.Location,
        };

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
            if (s_parser != null && s_cachedUntil < DateTime.UtcNow)
                return s_parser;

            if (!EveMonClient.IsDebugBuild && Dispatcher.IsMultiThreaded)
                GetExternalParser();

            s_cachedUntil = s_cachedUntil.AddHours(12);

            return new InternalEveNotificationTextParser();
        }

        /// <summary>
        /// Gets the external parser.
        /// </summary>
        private static void GetExternalParser()
        {
            if (s_queryPending)
                return;

            EveMonClient.Trace("EveNotificationTextParser.GetExternalParser - begin");

            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BitBucketWikiBase,
                NetworkConstants.EveNotificationTextExternalParser));

            HttpWebService.DownloadStringAsync(url, OnDownloaded, null);

            s_queryPending = true;
        }

        /// <summary>
        /// Processes the queried notification text parser.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="userstate">The userstate.</param>
        private static void OnDownloaded(DownloadStringAsyncResult e, object userstate)
        {
            if (e.Error != null)
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace("EveNotificationTextParser.GetExternalParser - failed");
                EveMonClient.Trace(e.Error.Message);
                return;
            }

            s_cachedUntil = s_cachedUntil.AddHours(12);

            s_parser = new CodeCompiler(s_referenceAssemblies).CreateInstanceFrom<EveNotificationTextParser>(e.Result);

            EveMonClient.Trace("EveNotificationTextParser.GetExternalParser - done");

            // Reset query pending flag
            s_queryPending = false;
        }
    }
}