using System;

namespace EVEMon.Common.Controls.MultiPanel
{
    /// <summary>
    /// Argument for the <see cref="MultiPanel.SelectionChange"/> event.
    /// </summary>
    public sealed class MultiPanelSelectionChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oldPage"></param>
        /// <param name="newPage"></param>
        public MultiPanelSelectionChangeEventArgs(MultiPanelPage oldPage, MultiPanelPage newPage)
        {
            OldPage = oldPage;
            NewPage = newPage;

        }

        /// <summary>
        /// Gets the old selection.
        /// </summary>
        public MultiPanelPage OldPage { get; private set; }

        /// <summary>
        /// Gets the new selection.
        /// </summary>
        public MultiPanelPage NewPage { get; private set; }
    }
}