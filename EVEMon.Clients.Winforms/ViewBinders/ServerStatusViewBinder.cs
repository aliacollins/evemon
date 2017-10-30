using EVEMon.Common;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EVEMon.Clients.Winforms.ViewBinders
{
    public static class ServerStatusViewBinder
    {
        private static List<ToolStripStatusLabel> legacyUIUpdates;
        private static String StatusTemplate = "|  Tranquility Server {0} ({1:n0} Pilots)";
        static ServerStatusViewBinder()
        {
            Common.Entities.ServerStatus.ServerStatus.registerForUpdate(onModelUpdateEvent);
            legacyUIUpdates = new List<ToolStripStatusLabel>();
        }
        
        // Called by the logic layer when the underlying data changes, and UI components need to be updated
        public static void onModelUpdateEvent()
        {
            updateLegacyUI();
        }

        private static void updateLegacyUI()
        {
            foreach (ToolStripStatusLabel uiComponent in legacyUIUpdates)
            {
                uiComponent.Text = String.Format(StatusTemplate, 
                    Common.Entities.ServerStatus.ServerStatus.CurrentServerMode.ToString(),
                    Common.Entities.ServerStatus.ServerStatus.PilotsOnline
                );
            }
        }

        public static void registerForLegacyUIUpdate(ToolStripStatusLabel uiComponent)
        {
            legacyUIUpdates.Add(uiComponent);
        }
    }
}
