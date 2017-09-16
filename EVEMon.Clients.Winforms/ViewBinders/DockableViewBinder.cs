using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVEMon.Clients.Winforms.ViewBinders
{
    public static class DockableViewBinder
    {
            private static List<Label> legacyUIUpdates;
            private static String DockedTemplate = "Docked at: {0}";
            static DockableViewBinder()
            {
                Common.Entities.Dockable.registerForUpdate(onModelUpdateEvent);
                legacyUIUpdates = new List<Label>();
            }

        // Called by the logic layer when the underlying data changes, and UI components need to be updated
            public static void onModelUpdateEvent(Int64 itemID)
            {
                updateLegacyUI(itemID);
            }

            private static void updateLegacyUI(Int64 itemID)
            {
                foreach (Label uiComponent in legacyUIUpdates)
                {
                    uiComponent.Text = String.Format(DockedTemplate,
                        Common.Entities.Dockable.getDockable(itemID).name
                    );
                }
            }

            public static void registerForLegacyUIUpdate(Label uiComponent)
            {
                legacyUIUpdates.Add(uiComponent);
            }
    }
}
