using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Extensions;
using EVEMon.Common.Service;
using YamlDotNet.RepresentationModel;

namespace EVEMon.Common.Models.Extended
{
    /// <summary>
    /// A class for the internal parser of the EVE notification text.
    /// </summary>
    /// <remarks>
    /// This class is called when the fetching of the external parser fails for any reason.
    /// </remarks>
    internal class InternalEveNotificationTextParser : EveNotificationTextParser
    {
        /// <summary>
        /// Parses the notification text.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="parsedDict">The parsed dictionary.</param>
        public override void Parse(EveNotification notification, KeyValuePair<YamlNode, YamlNode> pair,
            IDictionary<string, string> parsedDict)
        {
            switch (pair.Key.ToString().ToUpperInvariant())
            {
                case "CHARID":
                case "SENDERCHARID":
                case "RECEIVERCHARID":
                case "OWNERID":
                case "LOCATIONOWNERID":
                case "DESTROYERID":
                case "INVOKINGCHARID":
                case "CORPID":
                case "PODKILLERID":
                case "NEWCEOID":
                case "OLDCEOID":
                {
                    parsedDict[pair.Key.ToString()] = EveIDToName.GetIDToName(pair.Value.ToString());
                    break;
                }
                case "CLONESTATIONID":
                case "CORPSTATIONID":
                case "LOCATIONID":
                {
                    parsedDict[pair.Key.ToString()] = Station.GetByID(int.Parse(pair.Value.ToString())).Name;
                    break;
                }
                case "SHIPTYPEID":
                case "TYPEID":
                {
                    parsedDict[pair.Key.ToString()] = StaticItems.GetItemByID(int.Parse(pair.Value.ToString())).Name;
                    break;
                }
                case "MEDALID":
                {
                    var medal = notification.CCPCharacter.CharacterMedals
                        .FirstOrDefault(x => x.ID.ToString() == pair.Value.ToString());

                    parsedDict[pair.Key.ToString()] = medal == null
                        ? EVEMonConstants.UnknownText
                        : medal.Title ?? EVEMonConstants.UnknownText;
                    parsedDict.Add("medalDescription", medal == null
                        ? EVEMonConstants.UnknownText
                        : medal.Description ?? EVEMonConstants.UnknownText);
                    break;
                }
                case "ENDDATE":
                case "STARTDATE":
                {
                    parsedDict[pair.Key.ToString()] = string.Format(CultureConstants.InvariantCulture,
                        "{0:dddd, MMMM d, yyyy HH:mm} (EVE Time)", long.Parse(pair.Value.ToString())
                            .WinTimeStampToDateTime());
                    break;
                }
                case "NOTIFICATION_CREATED":
                {
                    parsedDict[pair.Key.ToString()] = string.Format(CultureConstants.InvariantCulture,
                        "{0:dddd, MMMM d, yyyy} (EVE Time)", long.Parse(pair.Value.ToString())
                            .WinTimeStampToDateTime());
                    break;
                }
                case "TYPEIDS":
                {
                    YamlSequenceNode typeIDs = pair.Value as YamlSequenceNode;

                    if (typeIDs == null)
                        break;

                    switch (notification.TypeID)
                    {
                        case 56:
                        case 57:
                        {
                            if (!typeIDs.Any())
                                parsedDict[pair.Key.ToString()] = "None were in the clone";
                            else
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (var typeID in typeIDs)
                                {
                                    sb.AppendLine()
                                        .AppendFormat("Type: {0}",
                                            StaticItems.GetItemByID(int.Parse(typeID.ToString())).Name)
                                        .AppendLine();
                                }
                                parsedDict[pair.Key.ToString()] = sb.ToString();
                            }
                        }
                            break;
                    }
                    break;
                }
                case "ISHOUSEWARMINGGIFT":
                {
                    if (!Convert.ToBoolean(pair.Value))
                        break;

                    switch (notification.TypeID)
                    {
                        case 34:
                            // Tritanium
                            parsedDict[pair.Key.ToString()] = StaticItems.GetItemByID(34).Name;
                            break;
                    }
                    break;
                }
            }
        }
    }
}