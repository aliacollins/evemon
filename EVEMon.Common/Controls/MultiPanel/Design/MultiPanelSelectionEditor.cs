using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace EVEMon.Common.Controls.MultiPanel.Design
{
    /// <summary>
    /// The editor for selecting the selected page property of the <see cref="MultiPanel"/>.
    /// </summary>
    internal class MultiPanelSelectionEditor : ObjectSelectorEditor
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        protected override void FillTreeWithData(Selector selection, ITypeDescriptorContext context, IServiceProvider provider)
        {
            // Base method, clear the selection
            base.FillTreeWithData(selection, context, provider);

            // Scroll through the pages
            MultiPanel panel = (MultiPanel)context.Instance;
            foreach (MultiPanelPage page in panel.Controls)
            {
                SelectorNode node = new SelectorNode(page.Name, page);
                selection.Nodes.Add(node);

                if (page != panel.SelectedPage)
                    continue;

                selection.SelectedNode = node;
                return;
            }
        }
    }
}
