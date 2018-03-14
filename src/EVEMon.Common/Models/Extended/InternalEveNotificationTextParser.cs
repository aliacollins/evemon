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
            string key = pair.Key.ToString(), value = pair.Value.ToString();
            switch (key.ToUpperInvariant())
            {
                case "CHARID":
                case "SENDERCHARID":
                case "RECEIVERCHARID":
                case "OWNERID":
                case "LOCATIONOWNERID":
                case "DESTROYERID":
                case "INVOKINGCHARID":
                case "PODKILLERID":
                case "NEWCEOID":
                case "OLDCEOID":
                {
                    parsedDict[key] = EveIDToName.CharIDToName(long.Parse(value));
                    break;
                }
                case "CORPID":
                {
                    parsedDict[key] = EveIDToName.CorpIDToName(long.Parse(value));
                    break;
                }
                case "CLONESTATIONID":
                case "CORPSTATIONID":
                case "LOCATIONID":
                {
                    parsedDict[key] = Station.GetByID(int.Parse(value)).Name;
                    break;
                }
                case "SHIPTYPEID":
                case "TYPEID":
                {
                    parsedDict[key] = StaticItems.GetItemByID(int.Parse(value)).Name;
                    break;
                }
                case "MEDALID":
                {
                    var medal = notification.CCPCharacter.CharacterMedals
                        .FirstOrDefault(x => x.ID.ToString() == value);

                    parsedDict[key] = medal == null
                        ? EveMonConstants.UnknownText
                        : medal.Title ?? EveMonConstants.UnknownText;
                    parsedDict.Add("medalDescription", medal == null
                        ? EveMonConstants.UnknownText
                        : medal.Description ?? EveMonConstants.UnknownText);
                    break;
                }
                case "ENDDATE":
                case "STARTDATE":
                {
                    parsedDict[key] = string.Format(CultureConstants.InvariantCulture,
                        "{0:dddd, MMMM d, yyyy HH:mm} (EVE Time)", long.Parse(value)
                            .WinTimeStampToDateTime());
                    break;
                }
                case "NOTIFICATION_CREATED":
                {
                    parsedDict[key] = string.Format(CultureConstants.InvariantCulture,
                        "{0:dddd, MMMM d, yyyy} (EVE Time)", long.Parse(value)
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
                                parsedDict[key] = "None were in the clone";
                            else
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (var typeID in typeIDs)
                                {
                                    sb
                                        .AppendLine()
                                        .AppendLine($"Type: {StaticItems.GetItemByID(int.Parse(typeID.ToString())).Name}");
                                }
                                parsedDict[key] = sb.ToString();
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
                            parsedDict[key] = StaticItems.GetItemByID(34).Name;
                            break;
                    }
                    break;
                }
                case "LEVEL":
                {
                    parsedDict[key] = $"{Standing.Status(double.Parse(value))} Standing";
                    break;
                }
            }
        }
    }
}
