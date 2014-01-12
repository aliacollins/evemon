using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace EVEMon.YamlToSql
{
    internal static class Program
    {
        private static string s_text;
        private static SqlConnection s_connection;
        private static DataTable s_dataTable;

        internal static void Main()
        {
            ConnectDB();
            ImportYamlFiles();
            DisconnectDB();
        }

        /// <summary>
        /// Connects the database.
        /// </summary>
        private static void ConnectDB()
        {
            s_text = "Connecting to SQL Database... ";
            Console.Write(s_text);

            s_connection = GetSqlConnection("EveStaticData");

            try
            {
                s_connection.Open();

                s_dataTable = s_connection.GetSchema("columns");

                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Connection to SQL Database: Successful");
                Console.WriteLine();
            }
            catch (SqlException)
            {
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Connection to SQL Database: Failed");
                Console.Write(@"Press any key to exit.");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Gets the SQL connection.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        private static SqlConnection GetSqlConnection(string connectionName)
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionName];
            if (connectionStringSetting != null)
                return new SqlConnection(connectionStringSetting.ConnectionString);

            Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
            Console.WriteLine(@"Can not find connection string with name: {0}", connectionName);
            Console.Write(@"Press any key to exit.");
            Console.ReadLine();
            Environment.Exit(-1);
            return null;
        }

        /// <summary>
        /// Imports the yaml files.
        /// </summary>
        private static void ImportYamlFiles()
        {
            ImportTypeIds();
            ImportGraphicIds();
            ImportIconIds();
        }

        /// <summary>
        /// Imports the type ids.
        /// </summary>
        private static void ImportTypeIds()
        {
            var yamlFile = YamlFilesConstants.typeIDs;
            var filePath = CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            var command = new SqlCommand { Connection = s_connection };

            const string TableName = "invTypes";
            if (s_dataTable.Select(String.Format("TABLE_NAME = '{0}'", TableName)).Length == 0)
            {
                Console.WriteLine(@"Can't find table '{0}'.", TableName);
                return;
            }

            s_text = String.Format(@"Importing {0}...", yamlFile);
            Console.WriteLine();
            Console.Write(s_text);

            TextReader tReader = new StreamReader(filePath);
            YamlStream yStream = new YamlStream();
            yStream.Load(tReader);
            YamlMappingNode rNode = yStream.Documents.First().RootNode as YamlMappingNode;

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            const string GraphicIDText = "graphicID";
            const string IconIDText = "iconID";
            const string RadiusText = "radius";
            const string SoundIDText = "soundID";

            CreateColumn(command, TableName, GraphicIDText, "int");
            CreateColumn(command, TableName, IconIDText, "int");
            CreateColumn(command, TableName, RadiusText, "float");
            CreateColumn(command, TableName, SoundIDText, "int");

            using (var tx = s_connection.BeginTransaction())
            {
                command.Transaction = tx;

                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        string id = ((YamlScalarNode)pair.Key).Value;
                        string graphicID = "null";
                        string iconID = "null";
                        string radius = "null";
                        string soundID = "null";

                        YamlMappingNode cNode = rNode.Children[new YamlScalarNode(id)] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        if (cNode.Children.ContainsKey(new YamlScalarNode(GraphicIDText)))
                        {
                            graphicID = ((YamlScalarNode)cNode.Children[new YamlScalarNode(GraphicIDText)]).Value;
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(IconIDText)))
                        {
                            iconID = ((YamlScalarNode)cNode.Children[new YamlScalarNode(IconIDText)]).Value;
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(RadiusText)))
                        {
                            radius = ((YamlScalarNode)cNode.Children[new YamlScalarNode(RadiusText)]).Value;
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(SoundIDText)))
                        {
                            soundID = ((YamlScalarNode)cNode.Children[new YamlScalarNode(SoundIDText)]).Value;
                        }

                        var parameters = new Dictionary<string, string>();
                        parameters[GraphicIDText] = graphicID;
                        parameters[IconIDText] = iconID;
                        parameters[RadiusText] = radius;
                        parameters[SoundIDText] = soundID;
                        parameters["columnFilter"] = "typeID";
                        parameters["id"] = id;

                        command.CommandText = SqlUpdateCommandText(TableName, parameters);

                        command.ExecuteNonQuery();
                    }

                    tx.Commit();

                    Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                    Console.WriteLine(@"{0} imported.", yamlFile);
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the graphic ids.
        /// </summary>
        private static void ImportGraphicIds()
        {
            var yamlFile = YamlFilesConstants.graphicsIDs;
            var filePath = CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            var command = new SqlCommand { Connection = s_connection };

            const string TableName = "eveGraphics";
            if (s_dataTable.Select(String.Format("TABLE_NAME = '{0}'", TableName)).Length == 0)
                CreateTable(command, TableName);
            else
                return;

            s_text = String.Format(@"Importing {0}...", yamlFile);
            Console.WriteLine();
            Console.Write(s_text);

            TextReader tReader = new StreamReader(filePath);
            YamlStream yStream = new YamlStream();
            yStream.Load(tReader);
            YamlMappingNode rNode = yStream.Documents.First().RootNode as YamlMappingNode;

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            const string GraphicFileText = "graphicFile";
            const string DescriptionText = "description";
            const string ObsoleteText = "obsolete";
            const string GraphicTypeText = "graphicType";
            const string CollidableText = "collidable";
            const string DirectoryIDText = "directoryID";
            const string GraphicNameText = "graphicName";
            const string GfxRaceIDText = "gfxRaceID";
            const string ColorSchemeText = "colorScheme";

            using (var tx = s_connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        string id = ((YamlScalarNode)pair.Key).Value;
                        string graphicFile = String.Format("'{0}'", String.Empty);
                        string description = String.Format("'{0}'", String.Empty);
                        string obsolete = "0";
                        string graphicType = "null";
                        string collidable = "null";
                        string directoryID = "null";
                        string graphicName = String.Format("'{0}'",String.Empty);
                        string gfxRaceID = "null";
                        string colorScheme = "null";

                        YamlMappingNode cNode = rNode.Children[new YamlScalarNode(id)] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        if (cNode.Children.ContainsKey(new YamlScalarNode(GraphicFileText)))
                        {
                            graphicFile = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(GraphicFileText)]).Value.Replace("'", "''"));
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(DescriptionText)))
                        {
                            description = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(DescriptionText)]).Value.Replace("'", "''"));
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(ObsoleteText)))
                        {
                            obsolete = ((YamlScalarNode)cNode.Children[new YamlScalarNode(ObsoleteText)]).Value == "true" ? "1" : "0";
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(GraphicTypeText)))
                        {
                            graphicType = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(GraphicTypeText)]).Value.Replace("'", "''"));
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(DirectoryIDText)))
                        {
                            directoryID = ((YamlScalarNode)cNode.Children[new YamlScalarNode(DirectoryIDText)]).Value;
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(CollidableText)))
                        {
                            collidable = ((YamlScalarNode)cNode.Children[new YamlScalarNode(CollidableText)]).Value == "true"
                                ? "1"
                                : "null";
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(GraphicNameText)))
                        {
                            graphicName = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(GraphicNameText)]).Value.Replace("'", "''"));
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(GfxRaceIDText)))
                        {
                            gfxRaceID = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(GfxRaceIDText)]).Value.Replace("'", "''"));
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(ColorSchemeText)))
                        {
                            colorScheme = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(ColorSchemeText)]).Value.Replace("'", "''"));
                        }

                        var parameters = new Dictionary<string, string>();
                        parameters["graphicID"] = id;
                        parameters[GraphicFileText] = graphicFile;
                        parameters[DescriptionText] = description;
                        parameters[ObsoleteText] = obsolete;
                        parameters[GraphicTypeText] = graphicType;
                        parameters[DirectoryIDText] = directoryID;
                        parameters[CollidableText] = collidable;
                        parameters[GraphicNameText] = graphicName;
                        parameters[GfxRaceIDText] = gfxRaceID;
                        parameters[ColorSchemeText] = colorScheme;

                        command.CommandText = SqlInsertCommandText(TableName, parameters);

                        command.ExecuteNonQuery();
                    }

                    tx.Commit();

                    Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                    Console.WriteLine(@"{0} imported.", yamlFile);
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the icon ids.
        /// </summary>
        private static void ImportIconIds()
        {
            var yamlFile = YamlFilesConstants.iconIDS;
            var filePath = CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            const string TableName = "eveIcons";
            var command = new SqlCommand { Connection = s_connection };

            if (s_dataTable.Select(String.Format("TABLE_NAME = '{0}'", TableName)).Length == 0)
                CreateTable(command, TableName);
            else
                return;

            s_text = String.Format(@"Importing {0}...", yamlFile);
            Console.WriteLine();
            Console.Write(s_text);

            TextReader tReader = new StreamReader(filePath);
            YamlStream yStream = new YamlStream();
            yStream.Load(tReader);
            YamlMappingNode rNode = yStream.Documents.First().RootNode as YamlMappingNode;

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }
            const string IconFileText = "iconFile";
            const string DescriptionText = "description";

            using (var tx = s_connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        string id = ((YamlScalarNode)pair.Key).Value;
                        string iconFile = String.Format("'{0}'", String.Empty);
                        string description = String.Format("'{0}'", String.Empty);

                        YamlMappingNode cNode = rNode.Children[new YamlScalarNode(id)] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        if (cNode.Children.ContainsKey(new YamlScalarNode(IconFileText)))
                        {
                            iconFile = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(IconFileText)]).Value.Replace("'", "''"));
                        }

                        if (cNode.Children.ContainsKey(new YamlScalarNode(DescriptionText)))
                        {
                            description = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[new YamlScalarNode(DescriptionText)]).Value.Replace("'", "''"));
                        }

                        var parameters = new Dictionary<string, string>();
                        parameters["iconID"] = id;
                        parameters[IconFileText] = iconFile;
                        parameters[DescriptionText] = description;

                        command.CommandText = SqlInsertCommandText(TableName, parameters);

                        command.ExecuteNonQuery();
                    }

                    tx.Commit();

                    Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                    Console.WriteLine(@"{0} imported.", yamlFile);
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// SQL insert command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private static string SqlInsertCommandText(string tableName, IDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            sb.Append("(");
            foreach (var parameter in parameters)
            {
                sb.Append(parameter.Key);
                if (!parameter.Equals(parameters.Last()))
                    sb.Append(", ");
            }
            sb.Append(") VALUES (");

            foreach (var parameter in parameters)
            {
                sb.Append(parameter.Value);
                sb.Append(!parameter.Equals(parameters.Last()) ? ", " : ")");
            }

            return String.Format("INSERT INTO {0} {1}", tableName, sb);
        }

        /// <summary>
        /// SQL update command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private static string SqlUpdateCommandText(string tableName, IDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            var pars = parameters.Where(par => par.Key != "columnFilter" && par.Key != "id")
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var parameter in pars)
            {
                sb.AppendFormat("{0} = {1}", parameter.Key, parameter.Value);
                if (!parameter.Equals(pars.Last()))
                    sb.Append(", ");
            }

            if (!String.IsNullOrWhiteSpace(parameters["columnFilter"]) && !String.IsNullOrWhiteSpace(parameters["id"]))
                sb.AppendFormat(" WHERE {0} = {1}", parameters["columnFilter"], parameters["id"]);

            return String.Format("UPDATE {0} SET {1}", tableName, sb);
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tableName">Name of the table.</param>
        private static void CreateTable(IDbCommand command, string tableName)
        {
            var filePath = CheckScriptFileExixts(tableName);

            string query;
            using (StreamReader sReader = new StreamReader(filePath))
            {
                query = sReader.ReadToEnd();
            }

            using (var tx = s_connection.BeginTransaction())
            {
                command.Transaction = tx;
                command.CommandText = query;

                try
                {
                    command.ExecuteNonQuery();
                    tx.Commit();
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Creates the column.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnType">Type of the column.</param>
        private static void CreateColumn(IDbCommand command, string tableName, string columnName, string columnType)
        {
            if (s_dataTable.Select(String.Format("COLUMN_NAME = '{0}' AND TABLE_NAME = '{1}'", columnName, tableName)).Length != 0)
                return;

            using (var tx = s_connection.BeginTransaction())
            {
                command.Transaction = tx;
                command.CommandText = String.Format("ALTER TABLE {0} ADD {1} {2} null", tableName, columnName, columnType);

                try
                {
                    command.ExecuteNonQuery();
                    tx.Commit();
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Checks the script file exixts.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private static string CheckScriptFileExixts(string tableName)
        {
            s_text = String.Format(@"Creating table {0}...", tableName);
            Console.WriteLine();
            Console.Write(s_text);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                String.Format(@"..{0}..{0}..{0}Scripts{0}{1}.table.sql", Path.DirectorySeparatorChar, tableName));

            if (File.Exists(filePath))
                return filePath;

            Console.WriteLine(@"{0}.table.sql file does not exists!", tableName);
            return String.Empty;
        }

        /// <summary>
        /// Checks the yaml file exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        private static string CheckYamlFileExists(string filename)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                String.Format(@"..{0}..{0}..{0}YamlFiles{0}{1}", Path.DirectorySeparatorChar, filename));

            if (File.Exists(filePath))
                return filePath;

            Console.WriteLine(@"{0} file does not exists!", filename);
            return String.Empty;
        }

        /// <summary>
        /// Disconnects the database.
        /// </summary>
        private static void DisconnectDB()
        {
            Console.WriteLine();

            try
            {
                s_connection.Close();

                Console.WriteLine(@"Disconnection from SQL Database: Successful");
                Console.Write(@"Press any key to exit.");
                Console.ReadLine();
            }
            catch (SqlException)
            {
                Console.WriteLine(@"Disconnection from SQL Database: Failed");
                Console.Write(@"Press any key to exit.");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }
    }
}
