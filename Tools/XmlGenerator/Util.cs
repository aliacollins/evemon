using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using EVEMon.Common;

namespace EVEMon.XmlGenerator
{
    public static class Util
    {
        private static string s_text;
        private static int s_counter;
        private static int s_tablesCount;
        private static int s_percentOld;

        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur.
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        /// <remarks>Currently unused, as we have switched to loading data from MSSQL, may be used in the future.</remarks>
        public static SimpleCollection<T> DeserializeList<T>(string filename)
        {
            return DeserializeXMLCore<SimpleCollection<T>>(filename);
        }

        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur.
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        /// <remarks>Currently unused, as we have switched to loading data from MSSQL, may be used in the future.</remarks>
        public static IndexedCollection<T> DeserializeIndexedList<T>(string filename)
            where T : IHasID
        {
            return DeserializeXMLCore<IndexedCollection<T>>(filename);
        }

        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur.
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        /// <remarks>Currently unused, as we have switched to loading data from MSSQL, may be used in the future.</remarks>
        public static Relations<T> DeserializeRelations<T>(string filename)
            where T : class, IRelation
        {
            return DeserializeXMLCore<Relations<T>>(filename);
        }

        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur.
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        /// <remarks>Currently unused, as we have switched to loading data from MSSQL, may be used in the future.</remarks>
        private static T DeserializeXMLCore<T>(string filename)
        {
            string path = Path.Combine(@"..\..\..\Input", filename);

            // Load xml doc
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            // Load XSLT 
            Assembly asm = Assembly.GetExecutingAssembly();
            XslCompiledTransform xslt = new XslCompiledTransform();
            Stream input = asm.GetManifestResourceStream(String.Format(CultureConstants.DefaultCulture,
                                                                       "{0}.Zofu.MySQLDumpImport.xslt", asm.GetName().Name));
            if (input != null)
            {
                using (XmlReader reader = XmlReader.Create(input))
                {
                    xslt.Load(reader);
                }
            }

            // Apply trasnform and deserialize
            using (XmlNodeReader reader = new XmlNodeReader(doc))
            {
                MemoryStream stream = Common.Util.GetMemoryStream();

                // Apply the XSL transform
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    xslt.Transform(reader, writer);
                    writer.Flush();

                    if (EveMonClient.IsDebugBuild)
                    {
                        // Gets a printing for debugging
                        stream.Seek(0, SeekOrigin.Begin);
                        XmlDocument doc2 = new XmlDocument();
                        doc2.Load(stream);
                        Trace.Write(Common.Util.GetXMLStringRepresentation(doc2));
                    }

                    // Deserialize from the given stream
                    stream.Seek(0, SeekOrigin.Begin);
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    T result = (T)xs.Deserialize(stream);

                    return result;
                }
            }
        }

        /// <summary>
        /// Serializes a XML file to EVEMon.Common\Resources.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datafile">The datafile.</param>
        /// <param name="filename">The filename.</param>
        internal static void SerializeXML<T>(T datafile, string filename)
        {
            string path = Path.Combine(@"..\..\..\..\..\EVEMon.Common\Resources", filename);

            FileStream stream = Common.Util.GetFileStream(path, FileMode.Create, FileAccess.Write);
            
            using (GZipStream zstream = new GZipStream(stream, CompressionMode.Compress))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(zstream, datafile);
                zstream.Flush();
            }

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Updated : {0}", filename);
            Console.WriteLine("-----------------------------------------------");

            // As long as EVEMon.Common is not rebuilt, files are not updated in output directories
            Copy(path, Path.Combine(@"..\..\..\..\..\EVEMon.Common\bin\x86\Debug\Resources", filename));
            Copy(path, Path.Combine(@"..\..\..\..\..\EVEMon.Common\bin\x86\Release\Resources", filename));
            Copy(path, Path.Combine(@"..\..\..\..\..\EVEMon\bin\x86\Debug\Resources", filename));
            Copy(path, Path.Combine(@"..\..\..\..\..\EVEMon\bin\x86\Release\Resources", filename));

            // Update the file in the settings directory
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Copy(path, Path.Combine(appData, Path.Combine("EVEMon", filename)));

            Console.WriteLine();
        }

        /// <summary>
        /// Serializes a XML file to EVEMon.Common\Serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serial">The serial.</param>
        /// <param name="xmlRootName">Name of the xml root.</param>
        /// <param name="filename">The filename.</param>
        internal static void SerializeXMLTo<T>(T serial, string xmlRootName, string filename)
        {
            string path = Path.Combine(@"..\..\..\..\..\EVEMon.Common\Serialization", filename);
            using (FileStream stream = Common.Util.GetFileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootName));
                serializer.Serialize(stream, serial);
                stream.Flush();
            }
        }

        /// <summary>
        /// Creates one file alongside the resources file containing
        /// the MD5 sums for each resource.
        /// </summary>
        /// <param name="filename">Filename of resource .xml.gz</param>
        internal static void CreateMD5SumsFile(string filename)
        {
            ResetCounters();

            const string ResourcesPath = @"..\..\..\..\..\EVEMon.Common\Resources";
            string md5SumsFileFullPath = Path.Combine(ResourcesPath, filename);

            using (StreamWriter md5SumsFile = File.CreateText(md5SumsFileFullPath))
            {
                foreach (string file in Directory.GetFiles(ResourcesPath, "*.xml.gz", SearchOption.TopDirectoryOnly))
                {
                    FileInfo datafile = new FileInfo(file);
                    if (!datafile.Exists)
                        throw new FileNotFoundException(String.Format(CultureConstants.DefaultCulture, "{0} not found!", file));

                    string line = String.Format(CultureConstants.DefaultCulture, "{0} *{1}", Common.Util.CreateMD5From(file),
                                                datafile.Name);
                    md5SumsFile.WriteLine(line);
                }
            }

            Console.WriteLine("MD5Sums File Created Successfully");
            Console.WriteLine();
        }

        /// <summary>
        /// Copies a file from source to destination.
        /// </summary>
        /// <param name="srcFile">Fully qualified source filename</param>
        /// <param name="destFile">Fully quallified destination filename</param>
        private static void Copy(string srcFile, string destFile)
        {
            try
            {
                FileInfo fi = new FileInfo(destFile);
                if (fi.Directory != null)
                {
                    if (fi.Directory.Exists && fi.Directory.Parent != null && fi.Directory.Parent.Parent != null)
                    {
                        File.Copy(srcFile, destFile, true);
                        Console.WriteLine(String.Format(CultureConstants.DefaultCulture, @"*** {0}\{1}\{2}",
                                                        fi.Directory.Parent.Parent.Name,
                                                        fi.Directory.Parent.Name, fi.Directory.Name));
                    }
                    else
                        Trace.WriteLine(String.Format(CultureConstants.DefaultCulture, "{0} doesn't exist, copy failed",
                                                      fi.Directory.FullName));
                }
            }
            catch (IOException exc)
            {
                WriteException(exc);
            }
            catch (UnauthorizedAccessException exc)
            {
                WriteException(exc);
            }
        }


        #region Helper Methods

        /// <summary>
        /// Writes the exception.
        /// </summary>
        /// <param name="exc">The exc.</param>
        private static void WriteException(Exception exc)
        {
            Trace.WriteLine(exc.ToString());
        }

        /// <summary>
        /// Resets the counters.
        /// </summary>
        internal static void ResetCounters()
        {
            if (Debugger.IsAttached)
                Console.WriteLine(s_counter);
            
            s_counter = 0;
            s_percentOld = 0;
            s_text = String.Empty;
        }

        /// <summary>
        /// Updates the percantage done of the datafile generating procedure.
        /// </summary>
        /// <param name="total"></param>
        internal static void UpdatePercentDone(int total)
        {
            s_counter++;
            int percent = (s_counter * 100 / total);

            if (s_counter != 1 && s_percentOld >= percent)
                return;

            if (!String.IsNullOrEmpty(s_text))
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);

            s_text = String.Format(CultureConstants.DefaultCulture, "{0}%", percent);
            Console.Write(s_text);
            s_percentOld = percent;
        }

        /// <summary>
        /// Updates the progress of data loaded from SQL server.
        /// </summary>
        internal static void UpdateProgress()
        {
            if (!String.IsNullOrEmpty(s_text))
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);

            s_tablesCount++;
            s_text = String.Format(CultureConstants.DefaultCulture, "{0}%", (s_tablesCount * 100 / Database.TotalTablesCount));
            Console.Write(s_text);
        }

        #endregion
    }
}