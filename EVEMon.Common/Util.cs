using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.BattleClinic;
using EVEMon.Common.Threading;

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
        public static XslCompiledTransform LoadXSLT(string content)
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
        ///Deserializes an XML document from a file.
        /// </summary>
        /// <typeparam name="T">The type to deserialize from the document</typeparam>
        /// <param name="filename">The XML document to deserialize from.</param>
        /// <param name="transform">The XSL transformation to apply. May be <c>null</c>.</param>
        /// <returns>The result of the deserialization.</returns>
        public static T DeserializeXMLFromFile<T>(string filename, XslCompiledTransform transform = null)
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
        /// Deserializes an XML document from a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static T DeserializeXMLFromString<T>(string text)
            where T : class
        {
            try
            {
                using (TextReader stream = new StringReader(text))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return (T)xs.Deserialize(stream);
                }
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

                    // Deserialization with transform
                    if (transform != null)
                    {
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

                    // Deserialization without transform
                    return (T)xs.Deserialize(gZipStream);
                }
            }
            catch (InvalidOperationException ex)
            {
                String message = String.Format(CultureConstants.DefaultCulture,
                                               "An error occurred decompressing {0}, the error message was '{1}' from '{2}'. "
                                               + "Try deleting all of the xml.gz files in %APPDATA%\\EVEMon.", filename,
                                               ex.Message, ex.Source);
                throw new InvalidOperationException(message, ex);
            }
            catch (XmlException ex)
            {
                String message = String.Format(CultureConstants.DefaultCulture,
                                               "An error occurred reading the XML from {0}, the error message was '{1}' from '{2}'. "
                                               + "Try deleting all of the xml.gz files in %APPDATA%\\EVEMon.", filename,
                                               ex.Message, ex.Source);
                throw new XmlException(message, ex);
            }
        }

        /// <summary>
        /// Deserialize an XML from a file.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="filename">The filename.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        /// <returns>The deserialized result</returns>
        internal static APIResult<T> DeserializeAPIResult<T>(string filename, XslCompiledTransform transform)
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
                return new APIResult<T>(exc);
            }
        }

        /// <summary>
        /// Asynchronously download an XML and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="url">The url to query</param>
        /// <param name="postData">The HTTP POST data to send, may be null.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        /// <param name="callback">The callback to call once the query has been completed.</param>
        internal static void DownloadAPIResultAsync<T>(Uri url, HttpPostData postData, XslCompiledTransform transform,
                                                       QueryCallback<T> callback)
        {
            EveMonClient.HttpWebService.DownloadXmlAsync(
                url, postData,
                (asyncResult, userState) =>
                    {
                        try
                        {
                            // Was there an HTTP error ?
                            APIResult<T> result = asyncResult.Error != null
                                                      ? new APIResult<T>(asyncResult.Error)
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
                null);
        }

        /// <summary>
        /// Synchronously download an XML and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="url">The url to query</param>
        /// <param name="postData">The HTTP POST data to send, may be null.</param>
        /// <param name="transform">The XSL transform to apply, may be null.</param>
        internal static APIResult<T> DownloadAPIResult<T>(Uri url, HttpPostData postData, XslCompiledTransform transform)
        {
            APIResult<T> result = new APIResult<T>(APIError.Http,
                                                   String.Format(CultureConstants.DefaultCulture, "Time out on querying {0}", url));

            // Query async and wait
            using (EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                EveMonClient.HttpWebService.DownloadXmlAsync(
                    url, postData,
                    (asyncResult, userState) =>
                        {
                            try
                            {
                                // Was there an HTTP error ?
                                result = asyncResult.Error != null
                                             ? new APIResult<T>(asyncResult.Error)
                                             : DeserializeAPIResultCore<T>(asyncResult.Result, transform);
                            }
                            catch (Exception e)
                            {
                                ExceptionHandler.LogException(e, true);
                                result = new APIResult<T>(APIError.Http, e.Message);
                                EveMonClient.Trace("Method: DownloadAPIResult, url: {0}, postdata: {1}, type: {2}",
                                                   url.AbsoluteUri, postData, typeof(T).Name);
                            }
                            finally
                            {
                                // We got the result, so we resume the calling thread
                                wait.Set();
                            }
                        },
                    null);

                // Wait for the completion of the background thread
                wait.WaitOne();
            }

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
        private static APIResult<T> DeserializeAPIResultCore<T>(IXPathNavigable doc, XslCompiledTransform transform)
        {
            APIResult<T> result;

            try
            {
                // Deserialization with a transform
                using (XmlNodeReader reader = new XmlNodeReader((XmlDocument)doc))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(APIResult<T>));

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
                            result = (APIResult<T>)xs.Deserialize(stream);
                        }
                    }
                        // Deserialization without transform
                    else
                        result = (APIResult<T>)xs.Deserialize(reader);
                }

                // Fix times
                DateTime requestTime = DateTime.UtcNow;
                double offsetCCP = (result.CurrentTime.Subtract(requestTime)).TotalMilliseconds;
                result.SynchronizeWithLocalClock(offsetCCP);
            }
                // An error occurred during the XSL transform
            catch (XsltException exc)
            {
                ExceptionHandler.LogException(exc, true);
                result = new APIResult<T>(exc);
            }
                // An error occurred during the deserialization
            catch (InvalidOperationException exc)
            {
                ExceptionHandler.LogException(exc, true);
                result = new APIResult<T>(exc);
            }
            catch (XmlException exc)
            {
                ExceptionHandler.LogException(exc, true);
                result = new APIResult<T>(exc);
            }

            // Stores XMLDocument
            result.XmlDocument = doc;
            return result;
        }

        /// <summary>
        /// Asynchronously download an XML and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The inner type to deserialize</typeparam>
        /// <param name="url">The url to query</param>
        /// <param name="postData">The HTTP POST data to send, may be null.</param>
        /// <param name="callback">The callback to call once the query has been completed.</param>
        internal static void DownloadBCAPIResultAsync<T>(Uri url, Serialization.BattleClinic.QueryCallback<T> callback,
                                                         HttpPostData postData = null)
        {
            EveMonClient.HttpWebService.DownloadXmlAsync(
                url, postData,
                (asyncResult, userState) =>
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
                    },
                null);
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
        /// <param name="postData">The http POST data to pass with the url. May be null.</param>
        /// <param name="callback">A callback invoked on the UI thread.</param>
        /// <returns></returns>
        public static void DownloadXMLAsync<T>(Uri url, DownloadCallback<T> callback, HttpPostData postData = null)
            where T : class
        {
            EveMonClient.HttpWebService.DownloadXmlAsync(
                url, postData,
                // Callback
                (asyncResult, userState) =>
                    {
                        T result = null;
                        string errorMessage = String.Empty;

                        // Was there an HTTP error ??
                        if (asyncResult.Error != null)
                            errorMessage = asyncResult.Error.Message;
                            // No http error, let's try to deserialize
                        else
                        {
                            try
                            {
                                // Deserialize
                                using (XmlNodeReader reader = new XmlNodeReader((XmlDocument)asyncResult.Result))
                                {
                                    XmlSerializer xs = new XmlSerializer(typeof(T));
                                    result = (T)xs.Deserialize(reader);
                                }
                            }
                                // An error occurred during the deserialization
                            catch (InvalidOperationException exc)
                            {
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
                null);
        }

        /// <summary>
        /// Gets a nicely formatted string representation of a XML document.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetXMLStringRepresentation(IXPathNavigable doc)
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
        /// <param name="serializationType">The type to pass to the <see cref="XmlSerializer"/></param>
        /// <param name="data">The object to serialize.</param>
        /// <returns>The Xml document representing the given object.</returns>
        public static IXPathNavigable SerializeToXmlDocument(Type serializationType, object data)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                // Serializes to the stream
                XmlSerializer serializer = new XmlSerializer(serializationType);
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
        /// Opens a file and look for a "revision" attribute and return its value.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>The revision number of the assembly which generated this file,
        /// or <c>0</c> if no such file was found (old format, before the introduction of the revision numbers).</returns>
        public static int GetRevisionNumber(string filename)
        {
            // Uses a regex to retrieve the revision number
            string content = File.ReadAllText(filename);
            Match match = Regex.Match(content, "revision=\"([0-9]+)\"", RegexOptions.Compiled);

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

            StringBuilder builder = new StringBuilder();

            Stream fileStream = GetFileStream(filename, FileMode.Open, FileAccess.Read);
            int bufferSize = Convert.ToInt32(fileStream.Length);
            using (Stream bufferedStream = new BufferedStream(fileStream, bufferSize))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(bufferedStream);
                    foreach (byte b in hash)
                    {
                        builder.Append(
                            b.ToString("x2", CultureConstants.InvariantCulture).ToLower(CultureConstants.InvariantCulture));
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets a memory stream.
        /// </summary>
        /// <returns>A new memory stream</returns>
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
    }
}
