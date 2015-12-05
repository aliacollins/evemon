using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Models;
using EVEMon.Common.Models.Extended;

namespace EVEMon.Common.Helpers
{
    public static class ApiTesterUIHelper
    {
        #region Fields

        private const string NoAPIKeyWithAccess = "No API key with access to API call found";

        private static readonly Enum[] s_apiMethodsHasIDOrName =
        {
            CCPAPIGenericMethods.CharacterID,
            CCPAPIGenericMethods.OwnerID,
            CCPAPIGenericMethods.CharacterName,
            CCPAPIGenericMethods.TypeName,
            CCPAPIGenericMethods.CharacterAffiliation,
            CCPAPIGenericMethods.ContractItems,
            CCPAPIGenericMethods.CorporationContractItems,
            CCPAPIGenericMethods.PlanetaryPins,
            CCPAPIGenericMethods.PlanetaryRoutes,
            CCPAPIGenericMethods.PlanetaryLinks,
            CCPAPICharacterMethods.CalendarEventAttendees,
            CCPAPICharacterMethods.Locations,
            CCPAPICharacterMethods.MailBodies,
            CCPAPICharacterMethods.NotificationTexts,
            CCPAPICorporationMethods.CorporationLocations,
            CCPAPICorporationMethods.CorporationStarbaseDetails
        };

        #endregion


        #region Properties

        /// <summary>
        /// Sets a value indicating whether we use internal info.
        /// </summary>
        /// <value>
        ///   <c>true</c> if we use internal info; otherwise, <c>false</c>.
        /// </value>
        public static bool UseInternalInfo { get; set; }

        /// <summary>
        /// Sets a value indicating whether we use external info.
        /// </summary>
        /// <value>
        ///   <c>true</c> if we use external info; otherwise, <c>false</c>.
        /// </value>
        public static bool UseExternalInfo { get; set; }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        public static object SelectedItem { get; set; }

        /// <summary>
        /// Sets the selected character.
        /// </summary>
        /// <value>
        /// The selected character.
        /// </value>
        public static object SelectedCharacter { get; set; }

        /// <summary>
        /// Sets the key ID.
        /// </summary>
        /// <value>
        /// The key ID.
        /// </value>
        public static string KeyID { get; set; }

        /// <summary>
        /// Sets the Verification code.
        /// </summary>
        /// <value>
        /// The Verification code.
        /// </value>
        public static string VCode { get; set; }

        /// <summary>
        /// Sets the char ID.
        /// </summary>
        /// <value>
        /// The char ID.
        /// </value>
        public static string CharID { get; set; }

        /// <summary>
        /// Sets the ID or name text.
        /// </summary>
        /// <value>
        /// The ID or name text.
        /// </value>
        public static string IDOrNameText { get; set; }

        /// <summary>
        /// Gets the error text.
        /// </summary>
        public static string ErrorText { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the API method has ID or name parameter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the API method has ID or name parameter; otherwise, <c>false</c>.
        /// </value>
        public static bool HasIDOrName
        {
            get { return SelectedItem != null && s_apiMethodsHasIDOrName.Any(method => SelectedItem.Equals(method)); }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public static Uri Url
        {
            get
            {
                string postData = GetPostData();
                string uriString = EveMonClient.APIProviders.CurrentProvider.GetMethodUrl((Enum)SelectedItem).AbsoluteUri;

                ErrorText = (postData == NoAPIKeyWithAccess ? NoAPIKeyWithAccess : String.Empty);

                if (!String.IsNullOrWhiteSpace(postData) && postData != NoAPIKeyWithAccess)
                    uriString += String.Format(CultureConstants.InvariantCulture, "?{0}", postData);

                return new Uri(uriString);
            }
        }

        /// <summary>
        /// Gets the API methods.
        /// </summary>
        public static IEnumerable<Enum> GetApiMethods
        {
            get
            {
                // List the API methods by type and name
                // Add the Server Status method on top
                List<Enum> apiMethods = new List<Enum> { CCPAPIGenericMethods.ServerStatus };

                // Add the non Account type methods
                apiMethods.AddRange(APIMethods.Methods.OfType<CCPAPIGenericMethods>().Where(
                    method => !apiMethods.Contains(method) &&
                              APIMethods.NonAccountGenericMethods.Contains(method)).Cast<Enum>().OrderBy(
                                  method => method.ToString()));

                // Add the Account type methods
                apiMethods.AddRange(APIMethods.Methods.OfType<CCPAPIGenericMethods>().Where(
                    method => !apiMethods.Contains(method) && !APIMethods.NonAccountGenericMethods.Contains(method) &&
                              !APIMethods.AllSupplementalMethods.Contains(method)).Cast<Enum>().OrderBy(
                                  method => method.ToString()));

                // Add the character methods
                apiMethods.AddRange(
                    APIMethods.Methods.OfType<CCPAPICharacterMethods>().Cast<Enum>().Concat(
                        APIMethods.CharacterSupplementalMethods).OrderBy(method => method.ToString()));

                // Add the corporation methods
                apiMethods.AddRange(
                    APIMethods.Methods.OfType<CCPAPICorporationMethods>().Cast<Enum>().Concat(
                        APIMethods.CorporationSupplementalMethods).OrderBy(method => method.ToString()));

                return apiMethods;
            }
        }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        public static IEnumerable<CCPCharacter> GetCharacters
        {
            get
            {
                return EveMonClient.Characters.OfType<CCPCharacter>().Where(
                    character => character.Identity.APIKeys.Any()).OrderBy(character => character.Name);
            }
        }

        #endregion


        #region Private Helper Methods

        /// <summary>
        /// Gets the post data.
        /// </summary>
        /// <returns></returns>
        private static string GetPostData()
        {
            if (SelectedItem is CCPAPIGenericMethods)
                return GetPostDataForGenericAPIMethods();

            if (SelectedItem is CCPAPICharacterMethods)
                return GetCharacterAPIMethodsPostData();

            if (SelectedItem is CCPAPICorporationMethods)
                return GetCorporationAPIMethodsPostData();

            return String.Empty;
        }

        /// <summary>
        /// Gets the post data for generic API methods.
        /// </summary>
        /// <returns></returns>
        private static string GetPostDataForGenericAPIMethods()
        {
            if (SelectedItem == null)
                return String.Empty;

            // Post data for character name, type name
            if (SelectedItem.Equals(CCPAPIGenericMethods.CharacterName) ||
                SelectedItem.Equals(CCPAPIGenericMethods.TypeName) ||
                SelectedItem.Equals(CCPAPIGenericMethods.CharacterAffiliation))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataIDsOnly, IDOrNameText);
            }

            // Post data for character id
            if (SelectedItem.Equals(CCPAPIGenericMethods.CharacterID) ||
                SelectedItem.Equals(CCPAPIGenericMethods.OwnerID))
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataNamesOnly, IDOrNameText);

            // Post data for supplemental API methods
            if (APIMethods.AllSupplementalMethods.Contains(SelectedItem))
                return SupplementalAPIMethodsPostData();

            // Post data for non account generic API methods
            if (APIMethods.NonAccountGenericMethods.Contains(SelectedItem))
                return String.Empty;

            if (UseInternalInfo)
            {
                if (SelectedCharacter == null)
                    return String.Empty;

                // Find associated API key 
                Character character = (Character)SelectedCharacter;
                APIKey apiKey = character.Identity.APIKeys.FirstOrDefault(key => key.IsCharacterOrAccountType);

                // No API key found else generic post data
                return apiKey == null
                    ? NoAPIKeyWithAccess
                    : String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                        apiKey.ID, apiKey.VerificationCode);
            }

            // Generic post data
            if (UseExternalInfo)
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                    KeyID, VCode);

            return String.Empty;
        }

        /// <summary>
        /// Gets the post data for the supplemental API methods.
        /// </summary>
        /// <returns></returns>
        private static string SupplementalAPIMethodsPostData()
        {
            if (SelectedItem == null)
                return String.Empty;

            if (UseInternalInfo)
            {
                if (SelectedCharacter == null)
                    return String.Empty;

                Character character = (Character)SelectedCharacter;
                APIKey apiKey = null;

                // Find associated API key for corporation contracts
                if (SelectedItem.ToString().StartsWith("CorporationContract", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICorporationMethods.CorporationContracts);

                // Find associated API key for corporation industry jobs history
                if (SelectedItem.ToString().StartsWith("CorporationIndustryJobsHistory", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICorporationMethods.CorporationIndustryJobs);

                // Find associated API key for corporation blueprints
                if (SelectedItem.ToString().StartsWith("CorporationBlueprints", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICorporationMethods.CorporationAssetList);

                // Find associated API key for character contracts
                if (SelectedItem.ToString().StartsWith("Contract", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.Contracts);

                // Find associated API key for character industry jobs history
                if (SelectedItem.ToString().StartsWith("IndustryJobsHistory", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.IndustryJobs);

                // Find associated API key for character planetary interaction
                if (SelectedItem.ToString().StartsWith("Planetary", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.AssetList);

                // Find associated API key for character blueprints
                if (SelectedItem.ToString().StartsWith("Blueprints", StringComparison.Ordinal))
                    apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.AssetList);

                // No API key found
                if (apiKey == null)
                    return NoAPIKeyWithAccess;

                // Post data for contract items
                if (SelectedItem.Equals(CCPAPIGenericMethods.ContractItems) ||
                    SelectedItem.Equals(CCPAPIGenericMethods.CorporationContractItems))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndContractID,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameText);
                }
                
                // Post data for planetary colonies
                if (SelectedItem.Equals(CCPAPIGenericMethods.PlanetaryColonies))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameText);
                }

                // Post data for planetary pins, routes, and links
                if (SelectedItem.Equals(CCPAPIGenericMethods.PlanetaryPins) ||
                    SelectedItem.Equals(CCPAPIGenericMethods.PlanetaryRoutes) ||
                    SelectedItem.Equals(CCPAPIGenericMethods.PlanetaryLinks))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndPlanetID,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameText);
                }

                // Generic post data
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                     apiKey.ID, apiKey.VerificationCode, character.CharacterID);
            }

            // Post data for contract items
            if (SelectedItem.Equals(CCPAPIGenericMethods.ContractItems) ||
                SelectedItem.Equals(CCPAPIGenericMethods.CorporationContractItems))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndContractID,
                                     KeyID, VCode, CharID, IDOrNameText);
            }

            // Post data for planetary pins, routes, and links
            if (SelectedItem.Equals(CCPAPIGenericMethods.PlanetaryPins) ||
                SelectedItem.Equals(CCPAPIGenericMethods.PlanetaryRoutes) ||
                SelectedItem.Equals(CCPAPIGenericMethods.PlanetaryLinks))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndPlanetID,
                                     KeyID, VCode, CharID, IDOrNameText);
            }

            // Generic post data
            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                 KeyID, VCode, CharID);
        }

        /// <summary>
        /// Gets the post data for character API methods.
        /// </summary>
        /// <returns></returns>
        private static string GetCharacterAPIMethodsPostData()
        {
            if (SelectedItem == null)
                return String.Empty;

            if (UseInternalInfo)
            {
                if (SelectedCharacter == null)
                    return String.Empty;

                // Find associated API key
                Character character = (Character)SelectedCharacter;
                APIKey apiKey = character.Identity.FindAPIKeyWithAccess((CCPAPICharacterMethods)SelectedItem);

                // No API key found
                if (apiKey == null)
                    return NoAPIKeyWithAccess;

                // Post data for character calendarEventAttendees, locations, mailBodies, notificationTexts
                if (SelectedItem.Equals(CCPAPICharacterMethods.CalendarEventAttendees) ||
                    SelectedItem.Equals(CCPAPICharacterMethods.Locations) ||
                    SelectedItem.Equals(CCPAPICharacterMethods.MailBodies) ||
                    SelectedItem.Equals(CCPAPICharacterMethods.NotificationTexts))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameText);
                }

                // Generic post data
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                     apiKey.ID, apiKey.VerificationCode, character.CharacterID);
            }

            // Post data for character info
            if (SelectedItem.Equals(CCPAPICharacterMethods.CharacterInfo) &&
                (KeyID.Length == 0 || VCode.Length == 0))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCharacterIDOnly,
                                     CharID);
            }

            // Post data for character calendarEventAttendees, locations, mailBodies, notificationTexts
            if (SelectedItem.Equals(CCPAPICharacterMethods.CalendarEventAttendees) || 
                SelectedItem.Equals(CCPAPICharacterMethods.Locations) ||
                SelectedItem.Equals(CCPAPICharacterMethods.MailBodies) ||
                SelectedItem.Equals(CCPAPICharacterMethods.NotificationTexts))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                     KeyID, VCode, CharID, IDOrNameText);
            }

            // Generic post data
            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharID,
                                 KeyID, VCode, CharID);
        }

        /// <summary>
        /// Gets the post data for corporation API methods.
        /// </summary>
        /// <returns></returns>
        private static string GetCorporationAPIMethodsPostData()
        {
            if (SelectedItem == null)
                return String.Empty;

            if (UseInternalInfo)
            {
                if (SelectedCharacter == null)
                    return String.Empty;

                // Find associated API key
                Character character = (Character)SelectedCharacter;
                APIKey apiKey = character.Identity.FindAPIKeyWithAccess((CCPAPICorporationMethods)SelectedItem);

                // No API key found
                if (apiKey == null)
                {
                    // Post data for simple corporation sheet
                    if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationSheet))
                        return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCorporationIDOnly,
                                             character.CorporationID);
                    return NoAPIKeyWithAccess;
                }

                // Post data for corporation location
                if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationLocations))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                         apiKey.ID, apiKey.VerificationCode, character.CharacterID, IDOrNameText);
                }

                // Post data for extended corporation member tracking
                if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationMemberTrackingExtended))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithExtendedParameter,
                                         apiKey.ID, apiKey.VerificationCode);
                }

                // Post data for corporation starbase details
                if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationStarbaseDetails))
                {
                    return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithItemID,
                                         apiKey.ID, apiKey.VerificationCode, IDOrNameText);
                }

                // Generic post data
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                     apiKey.ID, apiKey.VerificationCode);
            }

            // Post data for simple corporation sheet
            if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationSheet) &&
                (KeyID.Length == 0 || VCode.Length == 0))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataCorporationIDOnly,
                                     CharID);
            }

            // Post data for corporation location
            if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationLocations))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithCharIDAndIDS,
                                     KeyID, VCode, CharID, IDOrNameText);
            }

            // Post data for extended corporation member tracking
            if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationMemberTrackingExtended))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithExtendedParameter,
                                     KeyID, VCode);
            }

            // Post data for corporation starbase details
            if (SelectedItem.Equals(CCPAPICorporationMethods.CorporationStarbaseDetails))
            {
                return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataWithItemID,
                                     KeyID, VCode, IDOrNameText);
            }

            // Generic post data
            return String.Format(CultureConstants.InvariantCulture, NetworkConstants.PostDataBase,
                                 KeyID, VCode);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Saves the document to the disk.
        /// </summary>
        public static void SaveDocument(string filename, IXPathNavigable xmlDocument)
        {
            if (xmlDocument == null)
                throw new ArgumentNullException("xmlDocument");

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = "xml";
                sfd.Filter = "XML (*.xml)|*.xml";
                sfd.FileName = filename;

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    string content = Util.GetXmlStringRepresentation(xmlDocument);

                    // Moves to the final file
                    FileHelper.OverwriteOrWarnTheUser(
                        sfd.FileName,
                        fs =>
                            {
                                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                                {
                                    writer.Write(content);
                                    writer.Flush();
                                    fs.Flush();
                                }
                                return true;
                            });
                }
                catch (IOException err)
                {
                    ExceptionHandler.LogException(err, true);
                    MessageBox.Show("There was an error writing out the file:\n\n" + err.Message,
                                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (XmlException err)
                {
                    ExceptionHandler.LogException(err, true);
                    MessageBox.Show("There was an error while converting to XML format.\r\nThe message was:\r\n" + err.Message,
                                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}