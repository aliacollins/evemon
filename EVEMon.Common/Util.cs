using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using EVEMon.Common.CloudStorageServices.BattleClinic;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Helpers;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.BattleClinic.CloudStorage;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Threading;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using YamlDotNet.RepresentationModel;

namespace EVEMon.Common
{
    /// <summary>
    /// A delegate for downloads.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="errorMessage"></param>
    public delegate void DownloadCallback<in T>(T result, string errorMessage);

    /// <summary>
    /// A collection of helper methods for downloads and deserialization.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Opens the provided url in a new process.
        /// </summary>
        /// <param name="url"></param>
        public static void OpenURL(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            try
            {
                Process.Start(url.AbsoluteUri);
            }
            catch (FileNotFoundException ex)
            {
                ExceptionHandler.LogException(ex, false);
            }
        }

        /// <summary>
        /// Loads an XSL transform with the provided name from the resources.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static XslCompiledTransform LoadXslt(string content)
        {
            using (StringReader stringReader = new StringReader(content))
            {
                XmlTextReader reader = new XmlTextReader(stringReader);
                XslCompiledTransform xslt = new XslCompiledTransform();

                xslt.Load(reader);
                return xslt;
            }
        }

        /// <summary>
        /// Deserializes an XML document from a file.
        /// </summary>
        /// <typeparam name="T">The type to deserialize from the document</typeparam>
        /// <param name="filename">The XML document to deserialize from.</param>
        /// <param name="transform">The XSL transformation to apply. May be <c>null</c>.</param>
        /// <returns>The result of the deserialization.</returns>
        public static T DeserializeXmlFromFile<T>(string filename, XslCompiledTransform transform = null)
            where T : class
        {
            try
            {
                if (transform != null)
                {
                    MemoryStream stream = GetMemoryStream();
                    using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        // Apply the XSL transform
                        writer.Formatting = Formatting.Indented;
                        transform.Transform(filename, writer);
                        writer.Flush();

                        // Deserialize from the given stream
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        stream.Seek(0, SeekOrigin.Begin);
                        return (T)xs.Deserialize(stream);
                    }
                }

                // Deserialization without transform
                using (Stream stream = FileHelper.OpenRead(filename, false))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return (T)xs.Deserialize(stream);
                }
            }
            catch (XsltException exc)
            {
                // An error occurred during the XSL transform
                ExceptionHandler.LogException(exc, true);
                return null;
            }
            catch (InvalidOperationException exc)
            {
                // An error occurred during the deserialization
                ExceptionHandler.LogException(exc, true);
                return null;
            }
            catch (XmlException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return null;
            }
        }

        /// <summary>
        /// Deserializes an XML document from a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">The text.</param>
        /// <param name="transform">The transform.</param>
        /// <returns>The result of the deserialization.</returns>
        public static T DeserializeXmlFromString<T>(string text, XslCompiledTransform transform = null)
            where T : class
        {
            try
            {
                if (transform != null)
                {
                    MemoryStream stream = GetMemoryStream();
                    using (TextReader textReader = new StringReader(text))
                    using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        XmlReader reader = new XmlTextReader(textReader);

                        // Apply the XSL transform
                        writer.Formatting = Formatting.Indented;
                        transform.Transform(reader, writer);
                        writer.Flush();

                        // Deserialize from the given stream
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        stream.Seek(0, SeekOrigin.Begin);
                        return (T)xs.Deserialize(stream);
                    }
                }

                using (TextReader textReader = new StringReader(text))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return (T)xs.Deserialize(textReader);
                }
            }
                // An error occurred during the XSL transform
            catch (XsltException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return null;
            }
                // An error occurred during the deserialization
            catch (InvalidOperationException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return null;
            }
            catch (XmlException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return null;
            }
        }

        /// <summary>
        /// Deserializes a datafile.
        /// </summary>
        /// <typeparam name="T">The type to deserialize from the datafile</typeparam>
        /// <param name="filename">The datafile name</param>
        /// <param name="transform"></param>
        /// <returns></returns>
        internal static T DeserializeDatafile<T>(string filename, XslCompiledTransform transform = null)
        {
            // Gets the full path
            string path = Datafile.GetFullPath(filename);
            try
            {
                using (Stream stream = FileHelper.OpenRead(path, false))
                {
                    GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
                    XmlSerializer xs = new XmlSerializer(typeof(T));

                    // Deserialization without transform
                    if (transform == null)
                        return (T)xs.Deserialize(gZipStream);

                    // Deserialization with transform
                    MemoryStream memoryStream = GetMemoryStream();
                    XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.UTF8);
                    XmlTextReader reader = new XmlTextReader(gZipStream);

                    // Apply the XSL transform
                    writer.Formatting = Formatting.Indented;
                    transform.Transform(reader, writer);
                    writer.Flush();

                    // Deserialize from the given stream
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return (T)xs.Deserialize(memoryStream);

                }
            }
            catch (InvalidOperationException ex)
            {
                String message = String.Format(CultureConstants.DefaultCulture,
                                               "An error occurred decompressing {0}, the error message was '{1}' from '{2}'. "
                                               + "Try deleting all of the {3} files in %APPDATA%\\EVEMon.", filename,
                                               ex.Message, ex.Source, Datafile.DatafilesExtension);
                throw new InvalidOperationException(message, ex);
            }
            catch (XmlException ex)
            {
                String message = String.Format(CultureConstants.DefaultCulture,
                                               "An error occurred reading the XML from {0}, the error message was '{1}' from '{2}'. "
                                               + "Try deleting all of the {3} files in %APPDATA%\\EVEMon.", filename,
                                               ex.Message, ex.Source, Datafile.DatafilesExtension);
                throw new XmlException(message, ex);
            }
        }

        /// <summary>
        /// Deserializes an XML from a text.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="text">The text.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        /// <returns>The deserialized result</returns>
        internal static CCPAPIResult<T> DeserializeAPIResultFromString<T>(string text, XslCompiledTransform transform = null)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                return DeserializeAPIResultCore<T>(doc, transform);
            }
            catch (XmlException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return new CCPAPIResult<T>(exc);
            }
        }

        /// <summary>
        /// Deserialize an XML from a file.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="filename">The filename.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        /// <returns>The deserialized result</returns>
        internal static CCPAPIResult<T> DeserializeAPIResultFromFile<T>(string filename, XslCompiledTransform transform = null)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                return DeserializeAPIResultCore<T>(doc, transform);
            }
            catch (XmlException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return new CCPAPIResult<T>(exc);
            }
        }

        /// <summary>
        /// Asynchronously download an XML and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="url">The url to query</param>
        /// <param name="callback">The callback to call once the query has been completed.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        internal static void DownloadAPIResultAsync<T>(Uri url, Models.QueryCallback<T> callback, bool acceptEncoded = false,
                                                       string postData = null, XslCompiledTransform transform = null)
        {
            HttpWebService.DownloadXmlAsync(
                url,
                (asyncResult, userState) =>
                    {
                        try
                        {
                            // Was there an HTTP error ?
                            CCPAPIResult<T> result = asyncResult.Error != null
                                                      ? new CCPAPIResult<T>(asyncResult.Error)
                                                      : DeserializeAPIResultCore<T>(asyncResult.Result, transform);

                            // We got the result, let's invoke the callback on this actor
                            Dispatcher.Invoke(() => callback.Invoke(result));
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.LogException(e, false);
                            EveMonClient.Trace("Method: DownloadAPIResultAsync, url: {0}, postdata: {1}, type: {2}",
                                               url.AbsoluteUri, postData, typeof(T).Name);
                        }
                    },
                null, HttpMethod.Post, acceptEncoded, postData);
        }

        /// <summary>
        /// Synchronously download an XML and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="url">The url to query</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        internal static CCPAPIResult<T> DownloadAPIResult<T>(Uri url, bool acceptEncoded = false,
                                                          string postData = null, XslCompiledTransform transform = null)
        {
            CCPAPIResult<T> result = new CCPAPIResult<T>(Enumerations.CCPAPI.CCPAPIErrors.Http,
                                                   string.Format(CultureConstants.DefaultCulture, "Time out on querying {0}", url));

            // Query async and wait
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset);
            HttpWebService.DownloadXmlAsync(
                url,
                (asyncResult, userState) =>
                    {
                        try
                        {
                            // Was there an HTTP error ?
                            result = asyncResult.Error != null
                                         ? new CCPAPIResult<T>(asyncResult.Error)
                                         : DeserializeAPIResultCore<T>(asyncResult.Result, transform);
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.LogException(e, true);
                            result = new CCPAPIResult<T>(Enumerations.CCPAPI.CCPAPIErrors.Http, e.Message);
                            EveMonClient.Trace("Method: DownloadAPIResult, url: {0}, postdata: {1}, type: {2}",
                                               url.AbsoluteUri, postData, typeof(T).Name);
                        }
                        finally
                        {
                            // We got the result, so we resume the calling thread
                            wait.Set();
                        }
                    },
                null, HttpMethod.Post, acceptEncoded, postData);

            // Wait for the completion of the background thread
            wait.WaitOne();
            wait.Close();

            // Returns
            return result;
        }

        /// <summary>
        /// Process XML document.
        /// </summary>
        /// <typeparam name="T">The type to deserialize from the document</typeparam>
        /// <param name="transform">The XSL transformation to apply. May be <c>null</c>.</param>
        /// <param name="doc">The XML document to deserialize from.</param>
        /// <returns>The result of the deserialization.</returns>
        private static CCPAPIResult<T> DeserializeAPIResultCore<T>(IXPathNavigable doc, XslCompiledTransform transform = null)
        {
            CCPAPIResult<T> result;

            try
            {
                // Deserialization with a transform
                using (XmlNodeReader reader = new XmlNodeReader((XmlDocument)doc))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(CCPAPIResult<T>));

                    if (transform != null)
                    {
                        MemoryStream stream = GetMemoryStream();
                        using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                        {
                            // Apply the XSL transform
                            writer.Formatting = Formatting.Indented;
                            transform.Transform(reader, writer);
                            writer.Flush();

                            // Deserialize from the given stream
                            stream.Seek(0, SeekOrigin.Begin);
                            result = (CCPAPIResult<T>)xs.Deserialize(stream);
                        }
                    }
                        // Deserialization without transform
                    else
                        result = (CCPAPIResult<T>)xs.Deserialize(reader);
                }

                // Fix times
                if (result.Result is ISynchronizableWithLocalClock)
                {
                    DateTime requestTime = DateTime.UtcNow;
                    double offsetCCP = (result.CurrentTime.Subtract(requestTime)).TotalMilliseconds;
                    result.SynchronizeWithLocalClock(offsetCCP);
                }
            }
                // An error occurred during the XSL transform
            catch (XsltException exc)
            {
                ExceptionHandler.LogException(exc, true);
                result = new CCPAPIResult<T>(exc);
            }
                // An error occurred during the deserialization
            catch (InvalidOperationException exc)
            {
                ExceptionHandler.LogException(exc, true);
                result = new CCPAPIResult<T>(exc);
            }
            catch (XmlException exc)
            {
                ExceptionHandler.LogException(exc, true);
                result = new CCPAPIResult<T>(exc);
            }

            // Stores XMLDocument
            result.XmlDocument = doc;
            return result;
        }

        /// <summary>
        /// Synchronously download an XML and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="url">The url to query</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="dataCompression">The data compression.</param>
        /// <returns></returns>
        internal static BCAPIResult<T> DownloadBCAPIResult<T>(Uri url, bool acceptEncoded = false, string postData = null,
                                                              DataCompression dataCompression = DataCompression.None)
        {
            string errorMessage = String.Format(CultureConstants.DefaultCulture, "Time out on querying {0}", url);
            BCAPIError error = new BCAPIError { ErrorCode = "0", ErrorMessage = errorMessage };
            BCAPIResult<T> result = new BCAPIResult<T> { Error = error };

            // Query async and wait
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset);
            HttpWebService.DownloadXmlAsync(
                url,
                (asyncResult, userState) =>
                    {
                        try
                        {
                            // Was there an HTTP error ?
                            if (asyncResult.Error != null)
                            {
                                errorMessage = asyncResult.Error.InnerException == null
                                                   ? asyncResult.Error.Message
                                                   : asyncResult.Error.InnerException.Message;
                                error.ErrorMessage = errorMessage;
                            }
                            else
                                result = DeserializeBCAPIResultCore<T>(asyncResult.Result);
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.LogException(e, true);
                            errorMessage = e.InnerException == null
                                               ? e.Message
                                               : e.InnerException.Message;
                            error.ErrorMessage = errorMessage;
                            EveMonClient.Trace("Method: DownloadBCAPIResult, url: {0}, postdata: {1}, type: {2}",
                                               url.AbsoluteUri, postData, typeof(T).Name);
                        }
                        finally
                        {
                            // We got the result, so we resume the calling thread
                            wait.Set();
                        }
                    },
                null, HttpMethod.Post, acceptEncoded, postData, dataCompression);

            // Wait for the completion of the background thread
            wait.WaitOne();
            wait.Close();

            // Returns
            return result;
        }

        /// <summary>
        /// Asynchronously download an XML and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="url">The url to query</param>
        /// <param name="callback">The callback to call once the query has been completed.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="dataCompression">The data compression.</param>
        internal static void DownloadBCAPIResultAsync<T>(Uri url, QueryCallback<T> callback,
                                                         bool acceptEncoded = false, string postData = null,
                                                         DataCompression dataCompression = DataCompression.None)
        {
            HttpWebService.DownloadXmlAsync(
                url,
                (asyncResult, userState) =>
                    {
                        try
                        {
                            BCAPIResult<T> result = asyncResult.Error != null
                                                        ? null
                                                        : DeserializeBCAPIResultCore<T>(asyncResult.Result);

                            string errorMessage = asyncResult.Error == null
                                                      ? String.Empty
                                                      : asyncResult.Error.InnerException == null
                                                            ? asyncResult.Error.Message
                                                            : asyncResult.Error.InnerException.Message;

                            // We got the result, let's invoke the callback on this actor
                            Dispatcher.Invoke(() => callback.Invoke(result, errorMessage));
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.LogException(e, false);
                            EveMonClient.Trace("Method: DownloadBCAPIResultAsync, url: {0}, postdata: {1}, type: {2}",
                                               url.AbsoluteUri, postData, typeof(T).Name);
                        }
                    },
                null, HttpMethod.Post, acceptEncoded, postData, dataCompression);
        }

        /// <summary>
        /// Process XML document.
        /// </summary>
        /// <typeparam name="T">The type to deserialize from the document</typeparam>
        /// <param name="doc">The XML document to deserialize from.</param>
        /// <returns>The result of the deserialization.</returns>
        private static BCAPIResult<T> DeserializeBCAPIResultCore<T>(IXPathNavigable doc)
        {
            BCAPIResult<T> result;

            try
            {
                // Deserialization
                using (XmlNodeReader reader = new XmlNodeReader((XmlDocument)doc))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(BCAPIResult<T>));
                    result = (BCAPIResult<T>)xs.Deserialize(reader);
                }
            }
                // An error occurred during the deserialization
            catch (InvalidOperationException exc)
            {
                ExceptionHandler.LogException(exc, true);
                BCAPIError error = new BCAPIError
                    {
                        ErrorMessage = exc.InnerException == null
                                           ? exc.Message
                                           : exc.InnerException.Message
                    };
                result = new BCAPIResult<T> { Error = error };
            }
            catch (XmlException exc)
            {
                ExceptionHandler.LogException(exc, true);
                BCAPIError error = new BCAPIError
                    {
                        ErrorMessage = exc.InnerException == null
                                           ? exc.Message
                                           : exc.InnerException.Message
                    };
                result = new BCAPIResult<T> { Error = error };
            }

            return result;
        }

        /// <summary>
        /// Asynchronously download an object from an XML stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The url to download from</param>
        /// <param name="callback">A callback invoked on the UI thread.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postData">The http POST data to pass with the url. May be null.</param>
        public static void DownloadXmlAsync<T>(Uri url, DownloadCallback<T> callback, bool acceptEncoded = false,
                                               string postData = null)
            where T : class
        {
            HttpWebService.DownloadXmlAsync(
                url,
                // Callback
                (asyncResult, userState) =>
                {
                    T result = null;
                    string errorMessage = String.Empty;

                    // Was there an HTTP error ??
                    if (asyncResult.Error != null)
                        errorMessage = asyncResult.Error.Message;
                    else
                    {
                        // No http error, let's try to deserialize
                        try
                        {
                            // Deserialize
                            using (XmlNodeReader reader = new XmlNodeReader((XmlDocument)asyncResult.Result))
                            {
                                XmlSerializer xs = new XmlSerializer(typeof(T));
                                result = (T)xs.Deserialize(reader);
                            }
                        }
                        catch (InvalidOperationException exc)
                        {
                            // An error occurred during the deserialization
                            ExceptionHandler.LogException(exc, true);
                            errorMessage = (exc.InnerException == null
                                ? exc.Message
                                : exc.InnerException.Message);
                        }
                        catch (XmlException exc)
                        {
                            ExceptionHandler.LogException(exc, true);
                            errorMessage = (exc.InnerException == null
                                ? exc.Message
                                : exc.InnerException.Message);
                        }
                    }

                    // We got the result, let's invoke the callback on this actor
                    Dispatcher.Invoke(() => callback.Invoke(result, errorMessage));
                },
                null, HttpMethod.Post, acceptEncoded, postData);
        }

        public static void DownloadJsonAsync<T>(Uri url, DownloadCallback<T> callback, bool acceptEncoded = false,
                                                string postData = null) 
            where T : class
        {
            HttpWebService.DownloadStringAsync(
                url,
                // Callback
                (asyncResult, userState) =>
                {
                    T result = null;
                    string errorMessage = String.Empty;
                    
                    // Was there an HTTP error ??
                    if (asyncResult.Error != null)
                        errorMessage = asyncResult.Error.Message;
                    else
                    {
                        // No http error, let's try to deserialize
                        try
                        {
                            // Deserialize
                            result = new JavaScriptSerializer().Deserialize<T>(asyncResult.Result);
                        }
                        catch (InvalidOperationException exc)
                        {
                            // An error occurred during the deserialization
                            ExceptionHandler.LogException(exc, true);
                            errorMessage = (exc.InnerException == null
                                ? exc.Message
                                : exc.InnerException.Message);
                        }
                    }

                    // We got the result, let's invoke the callback on this actor
                    Dispatcher.Invoke(() => callback.Invoke(result, errorMessage));

                },
                null, HttpMethod.Post, acceptEncoded, postData);
        }

        /// <summary>
        /// Gets a nicely formatted string representation of a XML document.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetXmlStringRepresentation(IXPathNavigable doc)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            // Creates the settings for the text writer
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, NewLineHandling = NewLineHandling.Replace };

            // Writes to a string builder
            StringBuilder xmlBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(xmlBuilder, settings))
            {
                XmlDocument xmlDoc = (XmlDocument)doc;
                xmlDoc.WriteContentTo(xmlWriter);
                xmlWriter.Flush();
            }
            return xmlBuilder.ToString();
        }

        /// <summary>
        /// Serializes the given object to a XML document, dealt as the provided type.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <returns>The Xml document representing the given object.</returns>
        public static IXPathNavigable SerializeToXmlDocument(object data)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                // Serializes to the stream
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(memStream, data);

                // Creates a XML doc from the stream
                memStream.Seek(0, SeekOrigin.Begin);
                XmlDocument doc = new XmlDocument();
                doc.Load(memStream);

                return doc;
            }
        }

        /// <summary>
        /// Applies a XSLT to a <see cref="XmlDocument"/> and returns another <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="doc">The source <see cref="XmlDocument"/></param>
        /// <param name="xslt">The XSLT to apply.</param>
        /// <returns>The transformed <see cref="XmlDocument"/>.</returns>
        public static IXPathNavigable Transform(IXPathNavigable doc, XslCompiledTransform xslt)
        {
            if (xslt == null)
                throw new ArgumentNullException("xslt");

            MemoryStream stream = GetMemoryStream();
            using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                // Apply the XSL transform
                writer.Formatting = Formatting.Indented;
                xslt.Transform(doc, writer);
                writer.Flush();

                // Reads the XML document from the given stream.
                stream.Seek(0, SeekOrigin.Begin);
                XmlDocument outDoc = new XmlDocument();
                outDoc.Load(stream);
                return outDoc;
            }
        }

        /// <summary>
        /// Opens a file or search the given string and look for a "revision" attribute and return its value.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>The revision number of the assembly which generated this file,
        /// or <c>0</c> if no such file was found (old format, before the introduction of the revision numbers).</returns>
        public static int GetRevisionNumber(string filename)
        {
            // Uses a regex to retrieve the revision number
            string content = File.Exists(filename) ? File.ReadAllText(filename) : filename;

            Match match = Regex.Match(content, "revision=\"([0-9]+)\"",
                                      RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            // No match ? Then there was no "revision" attribute, this is an old format
            if (!match.Success || match.Groups.Count < 2)
                return 0;

            // Returns the revision number (first group is the whole match, the second one the capture)
            int revision;
            return Int32.TryParse(match.Groups[1].Value, out revision) ? revision : default(int);
        }

        /// <summary>
        /// Uncompress the given gzipped file to a temporary file and returns its filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>The temporary file's name.</returns>
        public static string UncompressToTempFile(string filename)
        {
            string tempFile = Path.GetTempFileName();

            // We decompress the gzipped stream and writes it to a temporary file
            FileStream stream = null;
            try
            {
                stream = File.OpenRead(filename);
                GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);

                using (FileStream outStream = File.OpenWrite(tempFile))
                {
                    byte[] bytes = new byte[4096];

                    // Since we're reading a compressed stream, the total number of bytes to decompress cannot be foreseen
                    // So we just continue reading while there are bytes to decompress
                    while (true)
                    {
                        int count = gzipStream.Read(bytes, 0, bytes.Length);
                        if (count == 0)
                            break;

                        outStream.Write(bytes, 0, count);
                    }

                    // Done, we flush and recall this method with the temp file name
                    outStream.Flush();
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
            return tempFile;
        }

        /// <summary>
        /// Gets the XML Root Element for the specified XML File.
        /// </summary>
        /// <remarks>
        /// After doing some testing, this is the fastest robust
        /// mechanism for getting the root node. This takes 480 ticks
        /// as opposed to > 900 for XmlDocument methods.
        /// </remarks>
        /// <param name="filename">Filename of an XmlDocument</param>
        /// <returns>Text representation of the root node</returns>
        public static string GetXmlRootElement(Uri filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename.LocalPath))
                throw new FileNotFoundException("Document not found", filename.LocalPath);

            using (XmlTextReader reader = new XmlTextReader(filename.LocalPath))
            {
                reader.XmlResolver = null;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        return reader.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the XML root element if the specified input is valid XML.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string GetXmlRootElement(TextReader input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(input);
                return xmlDoc.DocumentElement != null ? xmlDoc.DocumentElement.Name : null;
            }
            catch (XmlException)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates an MD5Sum of the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static string CreateMD5From(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(String.Format(CultureConstants.DefaultCulture, "{0} not found!", filename));

            Stream fileStream = GetFileStream(filename, FileMode.Open, FileAccess.Read);
            return CreateMD5(fileStream);
        }

        /// <summary>
        /// Creates an MD5Sum from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static string CreateMD5(Stream stream)
        {
            using (stream)
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
            }
        }

        /// <summary>
        /// Creates an SHA1Sum from the mac address of the first operational network interface.
        /// </summary>
        /// <returns></returns>
        public static string CreateSHA1SumFromMacAddress()
        {
            NetworkInterface ni = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up);

            if (ni == null)
                return String.Empty;

            Stream stream = GetMemoryStream(ni.GetPhysicalAddress().GetAddressBytes());
            return CreateSHA1(stream);
        }

        /// <summary>
        /// Creates an SHA1Sum from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static string CreateSHA1(Stream stream)
        {
            using (stream)
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets a memory stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>
        /// A new memory stream
        /// </returns>
        public static MemoryStream GetMemoryStream(byte[] buffer = null)
        {
            return buffer == null ? new MemoryStream() : new MemoryStream(buffer);
        }

        /// <summary>
        /// Gets a file stream.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <param name="share">The share.</param>
        /// <returns>A new file stream</returns>
        public static FileStream GetFileStream(string filePath, FileMode mode = FileMode.OpenOrCreate,
                                               FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None)
        {
            return new FileStream(filePath, mode, access, share);
        }

        /// <summary>
        /// Compresses the provided input data using zlib gzip.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns></returns>
        public static IEnumerable<byte> GZipCompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData");

            using (MemoryStream outputStream = GetMemoryStream())
            {
                GZipOutputStream gZipOutputStream = new GZipOutputStream(outputStream);
                gZipOutputStream.Write(inputData, 0, inputData.Length);
                gZipOutputStream.Finish();

                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Uncompresses the provided input data using zlib gzip.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns></returns>
        public static IEnumerable<byte> GZipUncompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData");

            using (MemoryStream inputStream = GetMemoryStream(inputData))
            using (MemoryStream outputStream = GetMemoryStream())
            {
                GZipInputStream gZipOutputStream = new GZipInputStream(inputStream);
                gZipOutputStream.CopyTo(outputStream);
                gZipOutputStream.Flush();

                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Compresses the provided input data using zlib deflater.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns></returns>
        public static IEnumerable<byte> DeflateCompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData");

            using (MemoryStream outputStream = GetMemoryStream())
            {
                DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(outputStream);
                deflaterOutputStream.Write(inputData, 0, inputData.Length);
                deflaterOutputStream.Finish();

                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Uncompresses the provided input data using zlib inflater.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns></returns>
        public static IEnumerable<byte> InflateUncompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData");

            using (MemoryStream inputStream = GetMemoryStream(inputData))
            using (MemoryStream outputStream = GetMemoryStream())
            {
                InflaterInputStream deflaterOutputStream = new InflaterInputStream(inputStream);
                deflaterOutputStream.CopyTo(outputStream);
                deflaterOutputStream.Flush();

                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Uncompress using zlib.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns></returns>
        public static IEnumerable<byte> ZlibUncompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData");

            if (!inputData.Any())
                return null;

            if (inputData[0] == 31 && inputData[1] == 139)
                return GZipUncompress(inputData);

            if (inputData[0] == 120 && inputData[1] == 156)
                return InflateUncompress(inputData);

            return inputData;
        }

        /// <summary>
        /// Uncompress using zlib.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns></returns>
        public static Stream ZlibUncompress(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            MemoryStream stream = inputStream as MemoryStream;

            if (stream == null)
                return inputStream;

            byte[] data = ZlibUncompress(stream.ToArray()) as byte[];

            return data == null ? inputStream : new MemoryStream(data);
        }

        /// <summary>
        /// Deserializes a JSON string to the passed object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static T DeserializeJson<T>(string json)
            where T : class
        {
            try
            {
                using (MemoryStream stream = GetMemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                    return (T)js.ReadObject(stream);
                }
            }
            catch (InvalidOperationException exc)
            {
                // An error occurred during the deserialization
                ExceptionHandler.LogException(exc, true);
                return null;
            }
            catch (SerializationException exc)
            {
                ExceptionHandler.LogException(exc, true);
                return null;
            }
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <param name="value">The json string.</param>
        /// <returns></returns>
        public static Dictionary<string, object> DeserializeJsonToObject(string value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Dictionary<string, object>>(value);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="jsonObj">The json object.</param>
        /// <returns></returns>
        public static string SerializeObjectToJson(Dictionary<string, object> jsonObj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(jsonObj);
        }

        /// <summary>
        /// Encrypts the specified text using the provided password.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string Encrypt(string text, string password)
        {
            // If no password is provided return the text unencrypted
            if (String.IsNullOrWhiteSpace(password))
                return text;

            // Ensure that salt is of the correct size
            while (password.Length < sizeof (long))
            {
                password += password;
            }

            byte[] encrypted;
            using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(password)))
            {
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    ICryptoTransform encryptor = aes.CreateEncryptor(pdb.GetBytes(32), pdb.GetBytes(16));
                    MemoryStream msEncrypt = GetMemoryStream();
                    CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write all data to the stream
                        swEncrypt.Write(text);
                        swEncrypt.Flush();
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts the specified ciphered text using the provided password.
        /// </summary>
        /// <param name="cipheredText">The ciphered text.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string Decrypt(string cipheredText, string password)
        {
            // If no password is provided return the text undecrypted
            if (String.IsNullOrWhiteSpace(password))
                return cipheredText;

            byte[] text;
            try
            {
                text = Convert.FromBase64String(cipheredText);
            }
                // If text is not encrypted return it undecrypted
            catch (FormatException)
            {
                return cipheredText;
            }

            // Ensure that salt is of the correct size
            while (password.Length < sizeof (long))
            {
                password += password;
            }

            string decrypted;
            using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(password)))
            {
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    try
                    {
                        ICryptoTransform decryptor = aes.CreateDecryptor(pdb.GetBytes(32), pdb.GetBytes(16));
                        MemoryStream msDecrypt = GetMemoryStream(text);
                        CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string
                            decrypted = srDecrypt.ReadToEnd();
                        }
                    }
                    catch (CryptographicException)
                    {
                        // In same rare cases the ciphered text may not be a ciphered one
                        // but still pass the Base64 convertion; In those cases the decryptor
                        // throws an exception, so we return the text as is
                        // Note: If anyone knows a better way to validate a string as base64 converted, please refactore the code
                        return cipheredText;
                    }
                }
            }
            return decrypted;
        }

        /// <summary>
        /// Parses the specified yaml text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        internal static YamlMappingNode ParseYaml(string text)
        {
            using (var sr = new StringReader(text))
            {
                YamlStream yStream = new YamlStream();
                yStream.Load(sr);
                return yStream.Documents.First().RootNode as YamlMappingNode;
            }
        }
    }
}
