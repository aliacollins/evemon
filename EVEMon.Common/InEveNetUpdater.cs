using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace EVEMon.Common
{
    public class InEveNetUpdater
    {
        /// <summary>
        /// Upload the character to InEve.net - Relies on the local xml cache.
        /// </summary>
        /// <param name="charName">Character name to update</param>
        public static void UpdateIneveAsync(string charName)
        {
            if (charName != null)
            {
                ThreadPool.QueueUserWorkItem(UpdateIneve, charName);
            }
        }

        /// <summary>
        /// Uploads the character to ineve.  Relies on the local xml cache.  Should only be called asynchronously.
        /// </summary>
        /// <param name="charName">Name of the char as an object.</param>
        protected static void UpdateIneve(object charName)
        {
            lock (LocalXmlCache.Instance)
            {
                string character = charName as string;
                WebClient client = new WebClient();
                byte[] bytes = null;
                try
                {
                    bytes = client.UploadFile("http://ineve.net/skills/evemon_upload.php", LocalXmlCache.Instance[character].FullName);
                }
                catch (WebException)
                {
                    //just fail and trust that we'll try again next time.
                    return;
                }
                string response = Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
