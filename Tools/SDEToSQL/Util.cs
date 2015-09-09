using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL
{
    internal static class Util
    {
        private static string s_text;
        private static int s_counter;
        private static int s_percentOld;


        /// <summary>
        /// Gets the SDE files directory.
        /// </summary>
        /// <value>
        /// The sde files directory.
        /// </value>
        private static string SDEFilesDirectory
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SDEFiles"); }
        }

        /// <summary>
        /// Updates the percent done.
        /// </summary>
        /// <param name="total">The total.</param>
        internal static void UpdatePercentDone(int total)
        {
            if (Program.IsClosing)
                return;

            s_counter = total > 0 && s_counter < total ? s_counter + 1 : s_counter;
            int percent = total > 0 ? (s_counter * 100 / total) : 0;

            if (s_counter < 0 || s_percentOld >= percent)
                return;

            s_percentOld = percent;

            if (!String.IsNullOrEmpty(s_text))
            {
                int position = Console.CursorLeft - s_text.Length;
                Console.SetCursorPosition(position > -1 ? position : 0, Console.CursorTop);
            }

            s_text = String.Format("{0}%", percent);

            if (Console.CursorLeft == 0 || Program.IsClosing)
                return;

            Console.Write(s_text);
        }

        /// <summary>
        /// Displays the end time.
        /// </summary>
        /// <param name="stopwatch">The stopwatch.</param>
        internal static void DisplayEndTime(Stopwatch stopwatch)
        {
            if (Program.IsClosing)
                return;

            Console.WriteLine(@" in {0}", stopwatch.Elapsed.ToString("g"));
        }

        /// <summary>
        /// Resets the counters.
        /// </summary>
        internal static void ResetCounters()
        {
            s_counter = 0;
            s_percentOld = -1;
            s_text = String.Empty;
        }

        /// <summary>
        /// Gets the script for the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        internal static string GetScriptFor(string tableName)
        {
            var resourceName = String.Format(@"{0}.Scripts.{1}.table.sql", typeof(Program).Namespace, tableName);

            string result = null;
            using (Stream stream = typeof(Program).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(result))
                return result;

            throw new MissingManifestResourceException(String.Format("{0}.table.sql resource file does not exists!", tableName));
        }

        /// <summary>
        /// Deletes the sde files.
        /// </summary>
        internal static void DeleteSDEFilesIfZipExists()
        {
            if (String.IsNullOrWhiteSpace(CheckSDEZipFileExists()))
                return;

            if (Program.IsClosing)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            foreach (
                string file in Directory.EnumerateFiles(SDEFilesDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => !x.EndsWith("_db.zip", StringComparison.OrdinalIgnoreCase) &&
                                !x.EndsWith("Put SDE zip or individual files here", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    File.Delete(file);
                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex.GetRecursiveStackTrace());
                }
            }
        }

        /// <summary>
        /// Checks the sde zip file exists.
        /// </summary>
        private static string CheckSDEZipFileExists()
        {
            var filePaths = Directory.EnumerateFiles(SDEFilesDirectory, "*_db.zip", SearchOption.TopDirectoryOnly).ToList();
            return filePaths.Count != 1 ? String.Empty : filePaths.Single();
        }

        /// <summary>
        /// Inflates the zip file.
        /// </summary>
        /// <param name="args">The arguments.</param>
        internal static void InflateZipFileIfExists(string[] args)
        {           
            if (!args.Any())
                InflateZipFile();
            else
            {
                if (args.Length == 1)
                {
                    if (args.All(x => x == "-norestore"))
                        InflateZipFile(includeFilter: "yaml | .db");

                    else if (args.All(x => x == "-noyaml"))
                        InflateZipFile(includeFilter: ".bak | .sql | .db");

                    else if (args.All(x => x == "-nosqlite"))
                        InflateZipFile(includeFilter: ".bak | .sql| .yaml");
                }

                if (args.Length != 2)
                    return;

                if (args.All(x => x != "-norestore"))
                    InflateZipFile(includeFilter: ".bak | .sql");

                else if (args.All(x => x != "-noyaml"))
                    InflateZipFile(includeFilter: ".yaml");

                else if (args.All(x => x != "-nosqlite"))
                    InflateZipFile(includeFilter: ".db");
            }
        }

        /// <summary>
        /// Inflates the zip file.
        /// </summary>
        /// <param name="includeFilter">The extraction filter.</param>
        private static void InflateZipFile(string includeFilter = null)
        {
            string zipFilePath = CheckSDEZipFileExists();

            if (String.IsNullOrWhiteSpace(zipFilePath))
                return;

            string s_text = @"SDE zip file found. Unzipping... ";
            Console.Write(s_text);

            try
            {
                // For more info about Shell usage visit
                // https://msdn.microsoft.com/en-us/library/windows/desktop/bb773938(v=vs.85).aspx

                Type shell = Type.GetTypeFromProgID("Shell.Application");

                dynamic objShell = Activator.CreateInstance(shell);
                dynamic destinationFolder = objShell.NameSpace(SDEFilesDirectory);
                dynamic sourceFile = objShell.NameSpace(zipFilePath);

                foreach (dynamic file in sourceFile.Items())
                {
                    if (Program.IsClosing)
                        break;

                    if (!String.IsNullOrWhiteSpace(includeFilter))
                    {
                        if (includeFilter.Split('|').All(filter =>
                            !file.Name.EndsWith(filter.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }
                    }

                    // For more info about Shell Folder usage visit
                    // https://msdn.microsoft.com/en-us/library/windows/desktop/bb787866(v=vs.85).aspx
                    // 4: Do not display a progress dialog box
                    // 16: Respond with "Yes to All" for any dialog box that is displayed
                    // 4096: Only operate in the local directory. Do not operate recursively into subdirectories

                    destinationFolder.CopyHere(file, 4 | 16 | 4096);
                }

                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Unzipping SDE zip file: Successful");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                HandleExceptionWithReason(s_text, @"Unzipping SDE zip file: Failed  ", ex.Message);
            }
        }

        /// <summary>
        /// Checks the data dump exists.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<string> CheckDataDumpExists()
        {
            string[] filePaths = Directory.GetFiles(SDEFilesDirectory, "DATADUMP*.bak");

            if (filePaths.Length == 1)
                return filePaths;

            Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
            Console.WriteLine(@"{0}", filePaths.Length == 0
                ? "Could not find the SDE data dump file in the 'SDEFiles' folder."
                : "More then one data dump has been found. Please restrict to only one.");
            PressAnyKey(-1);

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Checks the yaml file exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        internal static string CheckYamlFileExists(string filename)
        {
            var filePath = Path.Combine(SDEFilesDirectory, filename);

            if (File.Exists(filePath))
                return filePath;

            Console.WriteLine(@"{0} file does not exists!", filename);
            Console.WriteLine();

            return String.Empty;
        }

        /// <summary>
        /// Parses the yaml file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        internal static YamlMappingNode ParseYamlFile(string filePath)
        {
            YamlMappingNode rNode;
            using (TextReader tReader = new StreamReader(filePath))
            {
                YamlStream yStream = new YamlStream();
                yStream.Load(tReader);
                rNode = yStream.Documents.First().RootNode as YamlMappingNode;
            }
            return rNode;
        }

        /// <summary>
        /// Gets the text or default string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isUnicode">if set to <c>true</c> [is unicode].</param>
        /// <returns></returns>
        internal static string GetTextOrDefaultString(this string text, string defaultValue = "Null", bool isUnicode = false)
        {
            return String.IsNullOrWhiteSpace(text)
                ? defaultValue
                : String.Equals(text, "false", StringComparison.OrdinalIgnoreCase) ||
                  String.Equals(text, "true", StringComparison.OrdinalIgnoreCase)
                    ? Convert.ToByte(Convert.ToBoolean(text)).ToString()
                    : String.Format("{0}'{1}'", isUnicode ? "N" : String.Empty, text.Replace("'", "''"));
        }

        /// <summary>
        /// Gets the text or default string.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isUnicode">if set to <c>true</c> [is unicode].</param>
        /// <returns></returns>
        internal static string GetTextOrDefaultString(this IDictionary<YamlNode, YamlNode> node, string text,
            string defaultValue = "Null", bool isUnicode = false)
        {
            if (node == null || String.IsNullOrWhiteSpace(text) || !node.ContainsKey(new YamlScalarNode(text)))
                return defaultValue != Convert.DBNull.ToString() ? defaultValue : String.Format("'{0}'", defaultValue);

            return node[new YamlScalarNode(text)].ToString().GetTextOrDefaultString(defaultValue, isUnicode);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="exception">The exception.</param>
        internal static void HandleExceptionForCommand(IDbCommand command, Exception exception)
        {
            Console.WriteLine();
            Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
            Console.WriteLine(exception.Message);
            PressAnyKey(-1);
        }

        /// <summary>
        /// Handles the exception and provides a reason message.
        /// </summary>
        /// <param name="displayedText">The displayed text.</param>
        /// <param name="message">The message.</param>
        /// <param name="reason">The reason.</param>
        internal static void HandleExceptionWithReason(string displayedText, string message, string reason)
        {
            if (!String.IsNullOrEmpty(displayedText))
            {
                int position = Console.CursorLeft - displayedText.Length;
                Console.SetCursorPosition(position > -1 ? position : 0, Console.CursorTop);
            }

            if (String.IsNullOrEmpty(displayedText))
                Console.WriteLine();

            Console.WriteLine(message);
            Console.WriteLine(@"Reason was: {0}", reason);
            PressAnyKey(-1);
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        internal static string GetDescription(this Enum item)
        {
            return GetAttribute<DescriptionAttribute>(item).Description;
        }

        /// <summary>
        /// Gets the attribute associated to the given enumeration item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static TAttribute GetAttribute<TAttribute>(this Enum item)
            where TAttribute : Attribute
        {
            if (item == null)
                throw new ArgumentNullException("item");

            MemberInfo[] members = item.GetType().GetMember(item.ToString());
            if (members.Length <= 0)
                return null;

            object[] attrs = members[0].GetCustomAttributes(typeof(TAttribute), false);
            if (attrs.Length > 0)
                return (TAttribute)attrs[0];

            return null;
        }

        /// <summary>
        /// Gets the value from description.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private static T GetValueFromDescription<T>(this string description)
        {
            var type = typeof(T);

            if (!type.IsEnum)
                throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(type);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(type);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Gets the SQL type or the default value of the yaml node of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The SQL type.</typeparam>
        /// <typeparam name="T1">The enumeration type.</typeparam>
        /// <param name="node">The yaml node.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        internal static T GetSqlTypeOrDefaultOfEnum<T, T1>(this YamlNode node, string defaultValue = "Null")
            where T : INullable, IComparable, IXmlSerializable, new()
            where T1 : struct
        {
            MethodInfo parseMethod;
            if (node == null || !typeof(T1).IsEnum)
            {
                if (defaultValue == "Null")
                    return (T)typeof(T).GetField("Null").GetValue(null);

                parseMethod = new T().GetType().GetMethod("Parse");
                if (parseMethod != null && defaultValue != "")
                    return (T)parseMethod.Invoke(null, new object[] { defaultValue });

                return (T)Activator.CreateInstance(typeof(T), defaultValue);
            }

            int value = (int)Enum.ToObject(typeof(T1), node.ToString().GetValueFromDescription<T1>());

            parseMethod = new T().GetType().GetMethod("Parse");
            if (parseMethod == null)
                return (T)Activator.CreateInstance(typeof(T), value.ToString());

            return (T)parseMethod.Invoke(null, new object[] { value.ToString() });
        }

        /// <summary>
        /// Gets the SQL type or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The node.</param>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        internal static T GetSqlTypeOrDefault<T>(this IDictionary<YamlNode, YamlNode> node, string text,
            string defaultValue = "Null")
            where T : INullable, IComparable, IXmlSerializable, new()
        {
            MethodInfo parseMethod;
            if ((node == null) || String.IsNullOrWhiteSpace(text) || !node.ContainsKey(new YamlScalarNode(text)))
            {
                if (defaultValue == "Null")
                    return (T)typeof(T).GetField("Null").GetValue(null);

                parseMethod = new T().GetType().GetMethod("Parse");
                if (parseMethod != null && defaultValue != "")
                    return (T)parseMethod.Invoke(null, new object[] { defaultValue });

                return (T)Activator.CreateInstance(typeof(T), defaultValue);
            }

            parseMethod = new T().GetType().GetMethod("Parse");
            if (parseMethod == null)
                return (T)Activator.CreateInstance(typeof(T), node[new YamlScalarNode(text)].ToString());

            return (T)parseMethod.Invoke(null, new object[] { node[new YamlScalarNode(text)].ToString() });
        }

        /// <summary>
        /// Converts the provided data to a datatable.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static DataTable ToDataTable(this IQueryable<object> data)
        {
            UpdatePercentDone(0);

            using (DataTable table = new DataTable())
            {
                try
                {
                    PropertyInfo[] properties = data.GetType().GetGenericArguments().First()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    foreach (PropertyInfo prop in properties)
                    {
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }

                    var total = data.Count();
                    total += (int)(total * 0.01);

                    foreach (var item in data)
                    {
                        UpdatePercentDone(total);

                        DataRow row = table.NewRow();

                        foreach (PropertyInfo prop in properties)
                        {
                            row[prop.Name] = prop.GetValue(item, null) ?? DBNull.Value;
                        }

                        table.Rows.Add(row);
                    }

                    return table;
                }
                catch(Exception)
                {
                    return new DataTable();
                }
            }
        }

        /// <summary>
        /// Displays a "Press any key to exit." message and waits for a key press.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        internal static void PressAnyKey(int exitCode = 0)
        {
            Program.IsClosing = true;

            Console.WriteLine();
            Console.Write(@"Press any key to exit.");
            Console.ReadKey(true);
            DeleteSDEFilesIfZipExists();
            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Starts the trace logging.
        /// </summary>
        internal static void StartTraceLogging()
        {
            Trace.AutoFlush = true;
            StreamWriter traceStream = File.CreateText("trace.txt");
            TextWriterTraceListener traceListener = new TextWriterTraceListener(traceStream);
            Trace.Listeners.Add(traceListener);
        }

        /// <summary>
        /// Gets the recursive stack trace.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        /// <value>The recursive stack trace.</value>
        internal static string GetRecursiveStackTrace(this Exception exception)
        {
            StringBuilder stackTraceBuilder = new StringBuilder();
            Exception ex = exception;

            stackTraceBuilder.Append(ex).AppendLine();

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                stackTraceBuilder.AppendLine().Append(ex).AppendLine();
            }

            // Remove project local path from message
            return stackTraceBuilder.ToString().RemoveProjectLocalPath();
        }

        /// <summary>
        /// Removes the project local path from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string RemoveProjectLocalPath(this string text)
        {
            return Regex.Replace(text, @"[a-zA-Z]+:\\.*\\(?=SDEToSQL)",
                String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}