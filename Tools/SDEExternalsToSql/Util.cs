using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql
{
    internal static class Util
    {
        private static string s_text;
        private static int s_counter;
        private static int s_percentOld;

        /// <summary>
        /// Updates the percent done.
        /// </summary>
        /// <param name="total">The total.</param>
        internal static void UpdatePercentDone(int total)
        {
            s_counter++;
            int percent = (s_counter * 100 / total);

            if (s_counter != 1 && s_percentOld >= percent)
                return;

            if (!String.IsNullOrEmpty(s_text))
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);

            s_text = String.Format("{0}%", percent);
            Console.Write(s_text);
            s_percentOld = percent;
        }

        /// <summary>
        /// Displays the end time.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        internal static void DisplayEndTime(DateTime startTime)
        {
            Console.WriteLine(@" in {0}", DateTime.Now.Subtract(startTime).ToString("g"));
        }

        /// <summary>
        /// Resets the counters.
        /// </summary>
        internal static void ResetCounters()
        {
            s_counter = 0;
            s_percentOld = 0;
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
        /// Checks the yaml file exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        internal static string CheckYamlFileExists(string filename)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                String.Format(@"YamlFiles{0}{1}", Path.DirectorySeparatorChar, filename));

            if (File.Exists(filePath))
                return filePath;

            Console.WriteLine(@"{0} file does not exists!", filename);
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
        /// Gets the value or default string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        internal static string GetValueOrDefaultString<T>(this T? obj) where T : struct
        {
            return obj.HasValue
                ? obj is Boolean
                    ? Convert.ToByte(obj.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture)
                    : obj.Value.ToString()
                : Database.DbNull;
        }

        /// <summary>
        /// Gets the text or default string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="isUnicode">if set to <c>true</c> [is unicode].</param>
        /// <returns></returns>
        internal static string GetTextOrDefaultString(this string text, bool isUnicode = false)
        {
            return String.IsNullOrWhiteSpace(text)
                ? Database.DbNull
                : String.Format("{0}'{1}'", isUnicode ? "N" : String.Empty, text.Replace("'", Database.StringEmpty));
        }

        /// <summary>
        /// Gets the text or default string.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isUnicode">if set to <c>true</c> [is unicode].</param>
        /// <returns></returns>
        internal static string GetTextOrDefaultString(this IDictionary<YamlNode, YamlNode> node, string text, string defaultValue = "NULL",
            bool isUnicode = false)
        {
            if (node == null || String.IsNullOrWhiteSpace(text) || !node.ContainsKey(new YamlScalarNode(text)))
                return defaultValue;

            string nodeText = node[new YamlScalarNode(text)].ToString();
            return String.Equals(nodeText, "false", StringComparison.InvariantCultureIgnoreCase) ||
                   String.Equals(nodeText, "true", StringComparison.InvariantCultureIgnoreCase)
                ? Convert.ToByte(Convert.ToBoolean(nodeText)).ToString()
                : String.Format("{0}'{1}'", isUnicode ? "N" : String.Empty, nodeText.Replace("'", Database.StringEmpty));
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="e">The e.</param>
        internal static void HandleException(IDbCommand command, Exception e)
        {
            Console.WriteLine();
            Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
            Console.WriteLine(e.Message);
            Console.ReadLine();
            Environment.Exit(-1);
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
        internal static T GetValueFromDescription<T>(this string description)
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
        /// Gets the value or default string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        internal static string GetValueOrDefaultString<T>(this YamlNode node)
        {
            if (node == null || !typeof(T).IsEnum)
                return Database.DbNull;

            var value = (int)Enum.ToObject(typeof(T), node.ToString().GetValueFromDescription<T>());

            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        internal static string GetExceptionMessage(Exception e)
        {
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }

            return e.Message;
        }
    }
}