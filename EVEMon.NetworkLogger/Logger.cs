using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common;

namespace EVEMon.NetworkLogger
{
    public class Logger : IDisposable
    {
        public static Logger StartLogging()
        {
            using (LoggerWindow f = new LoggerWindow())
            {
                f.ShowDialog();
                if (f.DialogResult == DialogResult.Cancel)
                {
                    return null;
                }

                Logger l = new Logger(f.FileName, f.LoggingLevel);
                return l;
            }
        }

        private Logger(string filename, LoggingLevel logLevel)
        {
            m_fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            m_xmlWriter = new XmlTextWriter(m_fileStream, Encoding.UTF8);
            m_xmlWriter.Formatting = Formatting.Indented;
            m_xmlWriter.IndentChar = '\t';
            m_xmlWriter.Indentation = 1;

            m_xmlWriter.WriteStartDocument();
            m_xmlWriter.WriteStartElement("evemon-netlog");

            m_loggingLevel = logLevel;

        }

        private void EveSession_NetworkLogEvent(object sender, NetworkLogEventArgs e)
        {
            switch (m_loggingLevel)
            {
                case LoggingLevel.NoUsernameOrPassword:
                    if (e.Url.Contains("username="))
                    {
                        e.Url = Regex.Replace(e.Url, "username=([^&]*)", "username=REMOVED");
                    }
                    if (e.Referer.Contains("username="))
                    {
                        e.Referer = Regex.Replace(e.Referer, "username=([^&]*)", "username=REMOVED");
                    }
                    goto case LoggingLevel.NoPassword;
                case LoggingLevel.NoPassword:
                    if (e.Url.Contains("password="))
                    {
                        e.Url = Regex.Replace(e.Url, "password=([^&]*)", "password=REMOVED");
                    }
                    if (e.Referer.Contains("password="))
                    {
                        e.Referer = Regex.Replace(e.Referer, "password=([^&]*)", "password=REMOVED");
                    }
                    break;
            }

            XmlSerializer xs = new XmlSerializer(typeof (NetworkLogEventArgs));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            xs.Serialize(m_xmlWriter, e, ns);
            m_xmlWriter.Flush();
            m_fileStream.Flush();
        }

        private FileStream m_fileStream;
        private XmlTextWriter m_xmlWriter;
        private LoggingLevel m_loggingLevel;

        #region IDisposable Members
        public void Dispose()
        {
            m_xmlWriter.WriteEndElement();
            m_xmlWriter.Flush();
            m_xmlWriter.Close();
            try
            {
                m_fileStream.Close();
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }
        }
        #endregion
    }

    public enum LoggingLevel
    {
        NoUsernameOrPassword,
        NoPassword,
        All
    }
}