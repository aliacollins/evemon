using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Diagnostics;
using EVEMon.XmlImporter.Zofu;
using EVEMon.XmlGenerator.Zofu;

namespace EVEMon.XmlImporter
{
    public static class Util
    {
        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        public static ZofuList<T> DeserializeList<T>(string filename)
        {
            return DeserializeXMLCore<ZofuList<T>>(filename);
        }

        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        public static ZofuIndexedList<T> DeserializeIndexedList<T>(string filename)
            where T : IHasID
        {
            return DeserializeXMLCore<ZofuIndexedList<T>>(filename);
        }

        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        public static ZofuRelations<T> DeserializeRelations<T>(string filename)
            where T : class, IRelation
        {
            return DeserializeXMLCore<ZofuRelations<T>>(filename);
        }

        /// <summary>
        /// Deserializes an XML, returning null when exceptions occur
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="filename">The file to deserialize from</param>
        /// <returns>The deserialized object when success, <c>null</c> otherwise.</returns>
        private static T DeserializeXMLCore<T>(string filename)
        {
            string path = Path.Combine(@"..\..\..\Input", filename);

            // Load xml doc
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            // Load XSLT 
            var asm = Assembly.GetExecutingAssembly();
            XslCompiledTransform xslt = new XslCompiledTransform();
            using (var reader = XmlReader.Create(asm.GetManifestResourceStream(asm.GetName().Name + ".Zofu.MySQLDumpImport.xslt")))
            {
                xslt.Load(reader);
            }

            // Apply trasnform and deserialize
            using (XmlNodeReader reader = new XmlNodeReader(doc))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    // Apply the XSL transform
                    using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        xslt.Transform(reader, writer);
                        writer.Flush();

#if DEBUG
                        // Gets a printing for debugging
                        stream.Seek(0, SeekOrigin.Begin);
                        XmlDocument doc2 = new XmlDocument();
                        doc2.Load(stream);
                        string xmlstr = GetXMLStringRepresentation(doc2);
#endif

                        // Deserialize from the given stream
                        stream.Seek(0, SeekOrigin.Begin);
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        var result = (T)xs.Deserialize(stream);

                        // XML deserialization creates a lot of garbage so we cleans up now to avoid wasting hundreds of MB 
                        // and OOM exceptions.
                        GC.Collect();
                        return result;

                    }
                }
            }
        }

        /// <summary>
        /// Serializes a XML file to EVEMon.Common\Resources.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="filename"></param>
        internal static void SerializeXML<T>(T datafile, string filename)
        {
            string path = Path.Combine(@"..\..\..\..\EVEMon.Common\Resources", filename);

            using(var stream = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                using (GZipStream zstream = new GZipStream(stream, CompressionMode.Compress))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(zstream, datafile);
                    zstream.Flush();
                }
            }

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Updated : " + filename);
            Console.WriteLine("-----------------------------------------------");

            // As long as EVEMon.Common is not rebuilt, files are not updated in output directories
            Copy(path, Path.Combine(@"..\..\..\..\EVEMon.Common\bin\x86\Debug\Resources", filename));
            Copy(path, Path.Combine(@"..\..\..\..\EVEMon.Common\bin\x86\Release\Resources", filename));
            Copy(path, Path.Combine(@"..\..\..\..\EVEMon\bin\x86\Debug\Resources", filename));
            Copy(path, Path.Combine(@"..\..\..\..\EVEMon\bin\x86\Release\Resources", filename));

            // Update the file in the settings directory
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Copy(path, Path.Combine(appData, Path.Combine("EVEMon", filename)));

            Console.WriteLine();
        }

        internal static void CreateMD5SumsFile(string filename )
        {
            StreamWriter MD5file;
            string path = @"..\..\..\..\EVEMon.Common\Resources",
                   file = String.Format("{0}{1}{2}", path, Path.DirectorySeparatorChar, filename);

            MD5file = File.CreateText(file);
            MD5file.WriteLine(CreateMD5From("eve-certificates.xml.gz"));
            MD5file.WriteLine(CreateMD5From("eve-geography.xml.gz"));
            MD5file.WriteLine(CreateMD5From("eve-items.xml.gz"));
            MD5file.WriteLine(CreateMD5From("eve-properties.xml.gz"));
            MD5file.WriteLine(CreateMD5From("eve-reprocessing.xml.gz"));
            MD5file.WriteLine(CreateMD5From("eve-skills.xml.gz"));
            MD5file.Close();

            Console.WriteLine("MD5Sums File Created Successfully");
            Console.WriteLine();
        }

        /// <summary>
        // Creates a MD5Sum from datafile
        /// </summary>
        private static string CreateMD5From(string datafile)
        {
            string path = @"..\..\..\..\EVEMon.Common\Resources",
                   filename = String.Format("{0}{1}{2}", path, Path.DirectorySeparatorChar, datafile);

            StringBuilder sb = new StringBuilder();
            if (File.Exists(filename))
            {
                MD5 md5 = MD5.Create();
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    foreach (byte b in md5.ComputeHash(fs))
                        sb.Append(b.ToString("x2").ToLower());
                }

                // Constract the fileline
                string fileline = String.Concat(sb.ToString(), " *", datafile);

                return fileline;
            }
            else throw new ApplicationException(datafile + " not found!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="destFile"></param>
        private static void Copy(string srcFile, string destFile)
        {
            try
            {
                FileInfo fi = new FileInfo(destFile);

                if (fi.Directory.Exists)
                {
                    File.Copy(srcFile, destFile, true);
                    Console.WriteLine("*** " + destFile);
                }
                else
                {
                    Trace.WriteLine("{0} dosn't exist, copy failed", fi.Directory.FullName);
                }
            }
            catch(Exception exc)
            {
                Trace.WriteLine(exc.ToString());
            }
        }

        /// <summary>
        /// Gets a nicely formatted string representation of a XML document.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetXMLStringRepresentation(XmlDocument doc)
        {
            // Creates the settings for the text writer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Replace;

            // Writes to a string builder
            StringBuilder xmlBuilder = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(xmlBuilder, settings);
            doc.WriteContentTo(xmlWriter);
            xmlWriter.Flush();

            return xmlBuilder.ToString();
        }
    }
}
