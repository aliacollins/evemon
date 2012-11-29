using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public static class KillLogExporter
    {
        /// <summary>
        /// Copies the kill info to clipboard.
        /// </summary>
        public static void CopyKillInfoToClipboard(KillLog killLog)
        {
            try
            {
                string killLogInfoText = ExportKillLogInfo(killLog);
                if (String.IsNullOrEmpty(killLogInfoText))
                {
                    MessageBox.Show("No kill info was available. Nothing has been copied to the clipboard.", "Copy",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Clipboard.SetText(killLogInfoText, TextDataFormat.Text);
                MessageBox.Show("The kill info have been copied to the clipboard.", "Copy", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (ExternalException ex)
            {
                // Occurs when another process is using the clipboard
                ExceptionHandler.LogException(ex, true);
                MessageBox.Show("Couldn't complete the operation, the clipboard is being used by another process. " +
                                "Wait a few moments and try again.");
            }
        }

        /// <summary>
        /// Exports the kill log info.
        /// </summary>
        /// <returns></returns>
        private static string ExportKillLogInfo(KillLog killLog)
        {
            if (killLog == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(killLog.KillTime.ToString("yyyy.MM.dd HH:mm:ss", CultureConstants.InvariantCulture)).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "Victim: {0}", killLog.Victim.Name).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "Corp: {0}", killLog.Victim.CorporationName).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "Alliance: {0}", killLog.Victim.AllianceName).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "Faction: {0}", killLog.Victim.FactionName).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "Destroyed: {0}", killLog.Victim.ShipTypeName).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "System: {0}", killLog.SolarSystem.Name).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "Security: {0:N1}", killLog.SolarSystem.SecurityLevel).AppendLine();
            sb.AppendFormat(CultureConstants.InvariantCulture, "Damage Taken: {0}", killLog.Victim.DamageTaken).AppendLine();

            sb.AppendLine();
            sb.AppendLine("Involved parties:");
            sb.AppendLine();

            foreach (SerializableKillLogAttackersListItem attacker in killLog.Attackers.OrderByDescending(x => x.DamageDone))
            {
                // Append info for NPC or player entities
                if (String.IsNullOrEmpty(attacker.Name))
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Name: {0} / {1}", attacker.ShipTypeName, attacker.CorporationName);
                else
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Name: {0}", attacker.Name);

                if (attacker.FinalBlow)
                    sb.Append(" (laid the final blow)");
                sb.AppendLine();

                // Append info only for player entities
                if (!String.IsNullOrEmpty(attacker.Name))
                {
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Security: {0:N1}", attacker.SecurityStatus).AppendLine();
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Corp: {0}", attacker.CorporationName).AppendLine();
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Alliance: {0}",
                                    attacker.AllianceName == "Unknown" ? "None" : attacker.AllianceName).AppendLine();
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Faction: {0}",
                                    attacker.FactionName == "Unknown" ? "None" : attacker.FactionName).AppendLine();
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Ship: {0}", attacker.ShipTypeName).AppendLine();
                    sb.AppendFormat(CultureConstants.InvariantCulture, "Weapon: {0}", attacker.WeaponTypeName).AppendLine();
                }

                sb.AppendFormat(CultureConstants.InvariantCulture, "Damage Done: {0}", attacker.DamageDone).AppendLine();
                sb.AppendLine();
            }

            if (killLog.Items.Any(x => x.QtyDestroyed != 0))
            {
                sb.AppendLine("Destroyed items:");
                sb.AppendLine();
                AppendDestroyedItems(sb, killLog.Items.Where(x => x.QtyDestroyed != 0));
                sb.AppendLine();
            }

            if (killLog.Items.Any(x => x.QtyDropped != 0))
            {
                sb.AppendLine("Dropped items:");
                sb.AppendLine();
                AppendDroppedItems(sb, killLog.Items.Where(x => x.QtyDropped != 0));
                sb.AppendLine();
            }

            sb.AppendLine("<-- Generated by EVEMon -->");

            return sb.ToString();
        }

        /// <summary>
        /// Appends the dropped items.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="droppedItems">The dropped items.</param>
        private static void AppendDroppedItems(StringBuilder sb, IEnumerable<SerializableKillLogItemListItem> droppedItems)
        {
            foreach (SerializableKillLogItemListItem droppedItem in droppedItems)
            {
                Item item = StaticItems.GetItemByID(droppedItem.TypeID);
                if (item == null)
                    continue;

                if (droppedItem.EVEFlag == 0)
                    continue;

                sb.AppendFormat(CultureConstants.InvariantCulture, "{0}{1} ({2})",
                                item.Name,
                                droppedItem.QtyDropped > 1
                                    ? String.Format(CultureConstants.InvariantCulture, ", Qty: {0}", droppedItem.QtyDropped)
                                    : String.Empty, droppedItem.InventoryText).AppendLine();

                // Append any items inside a container
                if (droppedItem.Items.Count != 0)
                    AppendDestroyedItems(sb, droppedItem.Items.Where(x => x.QtyDropped != 0));
            }
        }

        /// <summary>
        /// Appends the destroyed items.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="destroyedItems">The destroyed items.</param>
        private static void AppendDestroyedItems(StringBuilder sb, IEnumerable<SerializableKillLogItemListItem> destroyedItems)
        {
            foreach (SerializableKillLogItemListItem destroyedItem in destroyedItems)
            {
                Item item = StaticItems.GetItemByID(destroyedItem.TypeID);
                if (item == null)
                    continue;

                if (destroyedItem.EVEFlag == 0)
                    continue;

                sb.AppendFormat(CultureConstants.InvariantCulture, "{0}{1} ({2})",
                                item.Name,
                                destroyedItem.QtyDestroyed > 1
                                    ? String.Format(CultureConstants.InvariantCulture, ", Qty: {0}", destroyedItem.QtyDestroyed)
                                    : String.Empty, destroyedItem.InventoryText).AppendLine();

                // Append any items inside a container
                if (destroyedItem.Items.Count != 0)
                    AppendDestroyedItems(sb, destroyedItem.Items.Where(x => x.QtyDestroyed != 0));
            }
        }
    }
}
