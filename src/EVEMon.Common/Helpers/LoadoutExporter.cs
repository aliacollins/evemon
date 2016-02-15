using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Helpers
{
    public static class LoadoutExporter
    {
        /// <summary>
        /// Exports to clipboard.
        /// </summary>
        /// <param name="loadoutInfo">The loadout information.</param>
        /// <param name="loadout">The loadout.</param>
        public static void ExportToClipboard(ILoadoutInfo loadoutInfo, Loadout loadout)
        {
            Dictionary<string, List<string>> items = GetItemsBySlots(loadout.Items);
            ExtractProperties(loadoutInfo, items);
            string exportText = SerializeToEFTFormat(loadoutInfo, loadout, items);

            // Copy to clipboard
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(exportText);
            }
            catch (ExternalException ex)
            {
                // There is a bug that results in an exception being
                // thrown when the clipboard is in use by another process
                ExceptionHandler.LogException(ex, true);
            }
        }

        /// <summary>
        /// Gets the items by slots.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        private static Dictionary<string, List<string>> GetItemsBySlots(IEnumerable<Item> items)
        {
            // Groups items by slots
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            foreach (IGrouping<string, Item> slotItems in items.GroupBy(LoadoutHelper.GetSlotByItem))
            {
                dictionary[slotItems.Key] = new List<string>();
                foreach (Item item in slotItems)
                {
                    dictionary[slotItems.Key].Add(item.Name);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Extracts the properties.
        /// </summary>
        /// <param name="loadoutInfo">The loadout information.</param>
        /// <param name="items">The items.</param>
        private static void ExtractProperties(ILoadoutInfo loadoutInfo, IDictionary<string, List<string>> items)
        {
            // Add "empty slot" mentions for every slot type
            foreach (EvePropertyValue prop in loadoutInfo.Ship.Properties.Where(prop => prop.Property != null))
            {
                // High Slots
                if (prop.Property.Name.Contains(LoadoutHelper.OrderedSlotNames[0]))
                {
                    int highSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty),
                        CultureConstants.InvariantCulture);

                    while (items.ContainsKey(LoadoutHelper.OrderedSlotNames[0]) && items[LoadoutHelper.OrderedSlotNames[0]].Count < highSlots)
                    {
                        items[LoadoutHelper.OrderedSlotNames[0]].Add("[empty high slot]");
                    }
                }
                // Medium Slots
                else if (prop.Property.Name.Contains(LoadoutHelper.OrderedSlotNames[1]))
                {
                    int medSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty),
                        CultureConstants.InvariantCulture);
                    while (items.ContainsKey(LoadoutHelper.OrderedSlotNames[1]) && items[LoadoutHelper.OrderedSlotNames[1]].Count < medSlots)
                    {
                        items[LoadoutHelper.OrderedSlotNames[1]].Add("[empty med slot]");
                    }
                }
                // Low Slots
                else if (prop.Property.Name.Contains(LoadoutHelper.OrderedSlotNames[2]))
                {
                    int lowSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty),
                        CultureConstants.InvariantCulture);
                    while (items.ContainsKey(LoadoutHelper.OrderedSlotNames[2]) && items[LoadoutHelper.OrderedSlotNames[2]].Count < lowSlots)
                    {
                        items[LoadoutHelper.OrderedSlotNames[2]].Add("[empty low slot]");
                    }
                }
                // Rig Slots
                else if (prop.Property.Name.Contains(LoadoutHelper.OrderedSlotNames[3]))
                {
                    int rigsSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty),
                        CultureConstants.InvariantCulture);
                    while (items.ContainsKey(LoadoutHelper.OrderedSlotNames[3]) && items[LoadoutHelper.OrderedSlotNames[3]].Count < rigsSlots)
                    {
                        items[LoadoutHelper.OrderedSlotNames[3]].Add("[empty rig slot]");
                    }
                }
                // Subsystem Slots
                else if (prop.Property.Name.Contains(LoadoutHelper.OrderedSlotNames[4]))
                {
                    int subSysSlots = Int32.Parse(Regex.Replace(prop.Value, @"[^\d]", String.Empty),
                        CultureConstants.InvariantCulture);
                    while (items.ContainsKey(LoadoutHelper.OrderedSlotNames[4]) && items[LoadoutHelper.OrderedSlotNames[4]].Count < subSysSlots)
                    {
                        items[LoadoutHelper.OrderedSlotNames[4]].Add(String.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Serializes to EFT format.
        /// </summary>
        /// <param name="loadoutInfo">The loadout information.</param>
        /// <param name="loadout">The loadout.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        private static string SerializeToEFTFormat(ILoadoutInfo loadoutInfo, Loadout loadout,
            IDictionary<string, List<string>> items)
        {
            // Build the output format for EFT
            StringBuilder exportText = new StringBuilder();
            exportText
                .AppendLine($"[{loadoutInfo.Ship.Name}, {loadout.Name} (EVEMon)]");

            // Low Slots
            if (items.ContainsKey(LoadoutHelper.OrderedSlotNames[2]))
            {
                exportText
                    .AppendLine(String.Join(Environment.NewLine, items[LoadoutHelper.OrderedSlotNames[2]].ToArray()))
                    .AppendLine();
            }

            // Medium Slots
            if (items.ContainsKey(LoadoutHelper.OrderedSlotNames[1]))
            {
                exportText
                    .AppendLine(String.Join(Environment.NewLine, items[LoadoutHelper.OrderedSlotNames[1]].ToArray()))
                    .AppendLine();
            }

            // High Slots
            if (items.ContainsKey(LoadoutHelper.OrderedSlotNames[0]))
            {
                exportText
                    .AppendLine(String.Join(Environment.NewLine, items[LoadoutHelper.OrderedSlotNames[0]].ToArray()))
                    .AppendLine();
            }

            // Rig Slots
            if (items.ContainsKey(LoadoutHelper.OrderedSlotNames[3]))
            {
                exportText
                    .AppendLine(String.Join(Environment.NewLine, items[LoadoutHelper.OrderedSlotNames[3]].ToArray()))
                    .AppendLine();
            }

            // Subsystem Slots
            if (items.ContainsKey(LoadoutHelper.OrderedSlotNames[4]))
            {
                exportText
                    .AppendLine(String.Join(Environment.NewLine, items[LoadoutHelper.OrderedSlotNames[4]].ToArray()))
                    .AppendLine();
            }

            // Drones
            if (items.ContainsKey(LoadoutHelper.OrderedSlotNames[6]))
            {
                foreach (IGrouping<string, string> itemName in items[LoadoutHelper.OrderedSlotNames[6]]
                    .GroupBy(itemName => itemName))
                {
                    exportText
                        .AppendLine($"{itemName.Key} x{itemName.Count()}");
                }
            }

            return exportText.ToString();
        }
    }
}

