using System;
using System.Drawing;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    /// <summary>
    /// A wrapper around <see cref="ToolTip"/> to use for infinite display.
    /// </summary>
    public class InfiniteDisplayToolTip : ToolTip
    {
        private readonly Control m_owner;
        private bool m_canceled;
        private Size m_size = new Size(0, 0);
        private string m_text = String.Empty;

        /// <summary>
        /// Initializes <see cref="InfiniteDisplayToolTip"/> instance.
        /// </summary>
        /// <param name="owner">Owner of this tooltip</param>
        public InfiniteDisplayToolTip(Control owner)
        {
            m_owner = owner;
            Popup += toolTip_Popup;
            UseFading = false;
        }

        /// <summary>
        /// Handles the Popup event of the toolTip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PopupEventArgs"/> instance containing the event data.</param>
        private void toolTip_Popup(object sender, PopupEventArgs e)
        {
            // If height of tooltip differs from the previous one, then we 
            // must store it and do not show tooltip at wrong position
            if (!m_size.Equals(e.ToolTipSize))
            {
                m_size = e.ToolTipSize;
                e.Cancel = true;
                m_canceled = true;
                m_text = String.Empty;
                return;
            }

            m_canceled = false;
        }

        /// <summary>
        /// Popups a tool tip with provided text, at the given point.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="pt">The pt.</param>
        public void Show(string text, Point pt)
        {
            if (text == m_text)
                return;

            m_text = text;
            Hide(m_owner);
            Show(text, m_owner, pt);

            // Cancel means new height and new position
            if (!m_canceled)
                return;

            Hide(m_owner);
            Show(text, m_owner, pt);
        }

        /// <summary>
        /// Hides a tooltip.
        /// </summary>
        public void Hide()
        {
            Hide(m_owner);
            m_text = String.Empty;
        }
    }
}