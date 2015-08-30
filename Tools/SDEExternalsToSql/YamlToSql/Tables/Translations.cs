using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal static class Translations
    {
        internal static string TranslationTableName
        {
            get { return "translationTables"; }
        }

        internal static string TrnTranslationColumnsTableName
        {
            get { return "trnTranslationColumns"; }
        }

        internal static string TrnTranslationsTableName
        {
            get { return "trnTranslations"; }
        }

        internal static string TranslationCategoriesID
        {
            get { return "6"; }
        }

        internal static string TranslationCategoriesGroupID
        {
            get { return "4"; }
        }

        internal static string TranslationGroupsID
        {
            get { return "7"; }
        }

        internal static string TranslationGroupsGroupID
        {
            get { return "5"; }
        }

        internal static string TranslationTypesGroupID
        {
            get { return TranslationGroupsGroupID; }
        }

        internal static string TranslationTypesDescriptionID
        {
            get { return "33"; }
        }

        internal static string TranslationTypesTypeNameID
        {
            get { return "8"; }
        }


        // translationTables and trnTranslationColumns and trnTranslations
        internal static string TcIDText
        {
            get { return "tcID"; }
        }

        // translationTables and trnTranslationColumns
        internal static string TcGroupIDText
        {
            get { return "tcGroupID"; }
        }


        // translationTables
        internal static string SourceTableText
        {
            get { return "sourceTable"; }
        }

        internal static string DestinationTableText
        {
            get { return "destinationTable"; }
        }

        internal static string TranslatedKeyText
        {
            get { return "translatedKey"; }
        }

        
        // trnTranslationColumns
        internal static string TableNameText
        {
            get { return "tableName"; }
        }

        internal static string ColumnNameText
        {
            get { return "columnName"; }
        }

        internal static string MasterIDText
        {
            get { return "masterID"; }
        }

        
        // trnTranslations
        internal static string KeyIDText
        {
            get { return "keyID"; }
        }

        internal static string LanguageIDText
        {
            get { return "languageID"; }
        }

        internal static string TextText
        {
            get { return "text"; }
        }


        internal static string EnglishLanguageIDText
        {
            get
            {
                return "en";
            }
        }

        internal static void ImportData(string tableName, IDictionary<string, string> parameters)
        {
            using (IDbCommand command = new SqlCommand(
                String.Empty,
                Database.SqlConnection,
                Database.SqlConnection.BeginTransaction()))
            {
                try
                {
                    command.CommandText = Database.SqlUpdateCommandText(tableName, parameters);
                    if (command.ExecuteNonQuery() == 0)
                    {
                        foreach (KeyValuePair<string, string> parameter in parameters
                            .Where(par => par.Key.StartsWith("id", StringComparison.OrdinalIgnoreCase) ||
                                          par.Key.StartsWith("columnFilter", StringComparison.OrdinalIgnoreCase))
                            .ToList())
                        {
                            parameters.Remove(parameter);
                        }

                        command.CommandText = Database.SqlInsertCommandText(tableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    command.Transaction.Rollback();
                    Util.HandleException(command, e);
                }
            }
        }

        /// <summary>
        /// Inserts the translations.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tcID">The translation column identifier.</param>
        /// <param name="keyId">The key identifier.</param>
        /// <param name="nameNodes">The name nodes.</param>
        internal static void InsertTranslations(IDbCommand command, string tcID, YamlNode keyId, YamlMappingNode nameNodes)
        {
            foreach (KeyValuePair<YamlNode, YamlNode> pair in nameNodes)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[TcIDText] = tcID;
                parameters[KeyIDText] = keyId.ToString();
                parameters[LanguageIDText] = GetProperLanguageID(pair.Key).GetTextOrDefaultString();
                parameters[TextText] = nameNodes.Children.GetTextOrDefaultString(pair.Key.ToString(), Database.StringEmpty, isUnicode: true);              
                parameters["id"] = parameters[TcIDText];
                parameters["columnFilter"] = TcIDText;
                parameters["id2"] = parameters[KeyIDText];
                parameters["columnFilter2"] = KeyIDText;
                parameters["id3"] = parameters[LanguageIDText];
                parameters["columnFilter3"] = LanguageIDText;

                command.CommandText = Database.SqlUpdateCommandText(TrnTranslationsTableName, parameters);
                if (command.ExecuteNonQuery() == 0)
                {
                    foreach (KeyValuePair<string, string> parameter in parameters
                        .Where(par => par.Key.StartsWith("id", StringComparison.OrdinalIgnoreCase) ||
                                      par.Key.StartsWith("columnFilter", StringComparison.OrdinalIgnoreCase))
                        .ToList())
                    {
                        parameters.Remove(parameter.Key);
                    }

                    command.CommandText = Database.SqlInsertCommandText(TrnTranslationsTableName, parameters);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string GetProperLanguageID(YamlNode key)
        {
            string languageIDText = key.ToString();
            if (String.Equals(languageIDText, EnglishLanguageIDText, StringComparison.InvariantCultureIgnoreCase))
                languageIDText = languageIDText + "-US";

            return languageIDText.ToUpperInvariant();
        }
    }
}
