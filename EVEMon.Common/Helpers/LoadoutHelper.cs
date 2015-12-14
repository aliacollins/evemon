using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Serialization.BattleClinic.Loadout;
using EVEMon.Common.Serialization.FittingXml;
using EVEMon.Common.Serialization.Osmium.Loadout;

namespace EVEMon.Common.Helpers
{
    public static class LoadoutHelper
    {
        /// <summary>
        /// Gets the ordered slot names.
        /// </summary>
        /// <value>
        /// The ordered slot names.
        /// </value>
        public static string[] OrderedSlotNames
        {
            get
            {
                return new[]
                {
                    "High Slots", "Med Slots", "Low Slots",
                    "Rig Slots", "Subsystem Slots", "Ammunition & Charges",
                    "Drones", "Unknown"
                };
            }
        }

        /// <summary>
        /// Determines whether the specified text is a loadout.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static bool IsLoadout(string text, out LoadoutFormat format)
        {
            if (IsEFTFormat(text))
            {
                format = LoadoutFormat.EFT;
                return true;
            }

            if (IsXMLFormat(text))
            {
                format = LoadoutFormat.XML;
                return true;
            }

            if (IsDNAFormat(text))
            {
                format = LoadoutFormat.DNA;
                return true;
            }

            format = LoadoutFormat.None;
            return false;
        }

        /// <summary>
        /// Determines whether the loadout is in EFT format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in EFT format; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEFTFormat(string text)
        {
            string[] lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Nothing to evaluate
            if (lines.Length == 0)
                return false;

            // Error on first line ?
            string line = lines.First();
            if (String.IsNullOrEmpty(line) || !line.StartsWith("[", StringComparison.CurrentCulture) || !line.Contains(","))
                return false;

            // Retrieve the ship
            int commaIndex = line.IndexOf(',');
            string shipTypeName = line.Substring(1, commaIndex - 1);

            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.Name == shipTypeName);
        }

        /// <summary>
        /// Determines whether the loadout is in XML format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in XML format; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsXMLFormat(string text)
        {
            XmlRootAttribute xmlRoot = new SerializableFittings().GetType().GetCustomAttributes(
                typeof(XmlRootAttribute), false).Cast<XmlRootAttribute>().FirstOrDefault();

            if (xmlRoot == null)
                return false;

            using (TextReader reader = new StringReader(text))
            {
                if (Util.GetXmlRootElement(reader) != xmlRoot.ElementName)
                    return false;
            }

            SerializableFittings fittings = Util.DeserializeXmlFromString<SerializableFittings>(text);
            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.Name == fittings.Fitting.ShipType.Name);
        }

        /// <summary>
        /// Determines whether the loadout is in DNA format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in DNA format; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDNAFormat(string text)
        {
            string[] lines = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Nothing to evaluate
            if (lines.Length == 0)
                return false;

            // Error on first line ?
            int shipID;
            string line = lines.First();
            if (String.IsNullOrEmpty(line) || !Int32.TryParse(line, out shipID))
                return false;

            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.ID == shipID);
        }

        /// <summary>
        /// Deserializes an EFT loadout text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static ILoadoutInfo DeserializeEFTFormat(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException("text");

            string[] lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            ILoadoutInfo loadoutInfo = new LoadoutInfo();

            // Nothing to evaluate
            if (lines.Length == 0)
                return loadoutInfo;

            var listOfItems = new List<Item>();
            Loadout loadout = null;

            foreach (
                string line in
                    lines.Where(line => !String.IsNullOrEmpty(line) && !line.Contains("empty") && !line.Contains("slot")))
            {
                // Retrieve the ship
                if (line == lines.First())
                {
                    // Retrieve the loadout name
                    int commaIndex = line.IndexOf(',');
                    loadoutInfo.Ship = StaticItems.GetItemByName(line.Substring(1, commaIndex - 1));

                    if (loadoutInfo.Ship == null)
                        return loadoutInfo;

                    loadout = new Loadout(line.Substring(commaIndex + 1, (line.Length - commaIndex - 2)).Trim(), String.Empty);

                    continue;
                }

                // Retrieve the item (might be a drone)
                string itemName = line.Contains(",") ? line.Substring(0, line.LastIndexOf(',')) : line;
                itemName = itemName.Contains(" x")
                    ? itemName.Substring(0, line.LastIndexOf(" x", StringComparison.CurrentCulture))
                    : itemName;

                Item item = StaticItems.GetItemByName(itemName) ?? Item.UnknownItem;

                listOfItems.Add(item);

                // Retrieve the charge
                string chargeName = line.Contains(",") ? line.Substring(line.LastIndexOf(',') + 2) : null;

                if (String.IsNullOrEmpty(chargeName))
                    continue;

                Item charge = StaticItems.GetItemByName(chargeName) ?? Item.UnknownItem;

                listOfItems.Add(charge);
            }

            if (loadout == null)
                return loadoutInfo;

            loadout.Items = listOfItems;
            loadoutInfo.Loadouts.Add(loadout);

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes the XML loadout text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static ILoadoutInfo DeserializeXMLFormat(string text)
        {
            SerializableFittings fittings = Util.DeserializeXmlFromString<SerializableFittings>(text);

            ILoadoutInfo loadoutInfo = new LoadoutInfo();

            // Nothing to evaluate
            if (fittings == null)
                return loadoutInfo;

            // Retrieve the ship
            loadoutInfo.Ship = StaticItems.GetItemByName(fittings.Fitting.ShipType.Name);

            if (loadoutInfo.Ship == null)
                return loadoutInfo;

            Loadout loadout = new Loadout(fittings.Fitting.Name, fittings.Fitting.Description.Text);

            IEnumerable<Item> listOfItems = fittings.Fitting.FittingHardware
                .Where(hardware => hardware != null && hardware.Item != null)
                .Select(hardware => hardware.Item);

            loadout.Items = listOfItems;
            loadoutInfo.Loadouts.Add(loadout);

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes the DNA loadout text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static ILoadoutInfo DeserializeDNAFormat(string text)
        {
            string[] lines = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            ILoadoutInfo loadoutInfo = new LoadoutInfo();

            // Nothing to evaluate
            if (lines.Length == 0)
                return loadoutInfo;

            var listOfItems = new List<Item>();
            Loadout loadout = null;

            foreach (string line in lines.Where(line => !String.IsNullOrEmpty(line)))
            {
                // Retrieve the ship
                if (line == lines.First())
                {
                    int shipID = Int32.Parse(line, CultureConstants.InvariantCulture);
                    loadoutInfo.Ship = StaticItems.GetItemByID(shipID);

                    if (loadoutInfo.Ship == null)
                        return loadoutInfo;

                    loadout = new Loadout(loadoutInfo.Ship.Name, String.Empty);

                    continue;
                }

                // Retrieve the item
                int itemID;
                Item item = Int32.TryParse(line.Substring(0, line.LastIndexOf(';')), out itemID)
                    ? StaticItems.GetItemByID(itemID) ?? Item.UnknownItem
                    : Item.UnknownItem;

                byte quantity = Byte.Parse(line.Substring(line.LastIndexOf(';') + 1), CultureConstants.InvariantCulture);

                for (int i = 0; i < quantity; i++)
                {
                    listOfItems.Add(item);
                }
            }

            if (loadout == null)
                return loadoutInfo;

            loadout.Items = listOfItems;
            loadoutInfo.Loadouts.Add(loadout);

            return loadoutInfo;
        }

        public static ILoadoutInfo DeserializeOsmiumJsonFeedFormat(Item ship, List<SerializableOsmiumLoadoutFeed> feed)
        {
            ILoadoutInfo loadoutInfo = new LoadoutInfo
            {
                Ship = ship
            };

            loadoutInfo.Loadouts
                .AddRange(feed
                    .Select(serialLoadout =>
                        new Loadout
                        {
                            ID = serialLoadout.ID,
                            Name = serialLoadout.Name,
                            Description = serialLoadout.RawDescription,
                            Author = serialLoadout.Author.Name,
                            Rating = serialLoadout.Rating,
                            SubmissionDate = serialLoadout.CreationDate.UnixTimeStampToDateTime(),
                            TopicUrl = new Uri(serialLoadout.Uri),
                            Items = Enumerable.Empty<Item>()
                        }));

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes the BattleClinic XML feed format.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="feed">The feed.</param>
        /// <returns></returns>
        public static ILoadoutInfo DeserializeBCXMLFeedFormat(Item ship, SerializableBCLoadoutFeed feed)
        {
            ILoadoutInfo loadoutInfo = new LoadoutInfo
            {
                Ship = ship
            };

            loadoutInfo.Loadouts
                .AddRange(feed.Race.Loadouts
                    .Select(serialLoadout =>
                        new Loadout
                        {
                            ID = serialLoadout.ID,
                            Name = serialLoadout.Name,
                            Description = String.Empty,
                            Author = serialLoadout.Author,
                            Rating = serialLoadout.Rating,
                            SubmissionDate = serialLoadout.SubmissionDate,
                            TopicUrl = new Uri(
                                String.Format(CultureConstants.InvariantCulture,
                                    NetworkConstants.BattleClinicLoadoutTopic, serialLoadout.TopicID)),
                            Items = Enumerable.Empty<Item>()
                        }));

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes the BattleClinic XML loadout format.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="slots">The slots.</param>
        public static void DeserializeBCXMLLoadoutFormat(Loadout loadout, IEnumerable<SerializableBCLoadoutSlot> slots)
        {
            var listOfItems = new List<Item>();

            foreach (IGrouping<string, SerializableBCLoadoutSlot> slotType in slots.GroupBy(x => x.SlotType))
            {
                listOfItems.AddRange(slotType.Where(slot => slot.ItemID != 0)
                    .Select(slot => StaticItems.GetItemByID(slot.ItemID))
                    .Where(item => item != null));
            }

            loadout.Items = listOfItems;
        }

        /// <summary>
        /// Gets the slot by item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string GetSlotByItem(Item item)
        {
            switch (item.FittingSlot)
            {
                // High Slot
                case ItemSlot.High:
                    return OrderedSlotNames[0];
                // Medium Slot
                case ItemSlot.Medium:
                    return OrderedSlotNames[1];
                // Low Slot
                case ItemSlot.Low:
                    return OrderedSlotNames[2];
            }

            // Rig Slot
            if (item.MarketGroup.BelongsIn(DBConstants.ShipModificationsMarketGroupID))
                return OrderedSlotNames[3];

            // Subsystems
            if (item.MarketGroup.BelongsIn(DBConstants.SubsystemsMarketGroupID))
                return OrderedSlotNames[4];

            // Ammunition & Charges
            if (item.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID))
                return OrderedSlotNames[5];

            // Drones
            if (item.MarketGroup.BelongsIn(DBConstants.DronesMarketGroupID))
                return OrderedSlotNames[6];

            return OrderedSlotNames[7];
        }
    }
}
