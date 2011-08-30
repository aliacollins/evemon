using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using EVEMon.Controls.MultiPanel.Design;

namespace EVEMon.Controls.MultiPanel
{
    public delegate void MultiPanelSelectionChangeHandler(object sender, MultiPanelSelectionChangeEventArgs args);

    /// <summary>
    /// A panel with multiple pages that can be switched.
    /// </summary>
    /// <remarks>
    /// Based on the work from Liron Levi on Code Project, under public domain. 
    /// See http://www.codeproject.com/KB/cs/multipanelcontrol.aspx
    /// </remarks>
    [Designer(typeof(MultiPanelDesigner))]
    public class MultiPanel : Panel
    {
        public event MultiPanelSelectionChangeHandler SelectionChange;

        private MultiPanelPage m_selectedPage;


        /// <summary>
        /// Gets or sets the selected page.
        /// </summary>
        [Category("Appearance")]
        [Description("The selected page.")]
        [Editor(typeof(MultiPanelSelectionEditor), typeof(UITypeEditor))]
        public MultiPanelPage SelectedPage
        {
            get { return m_selectedPage; }
            set
            {
                if (m_selectedPage == value)
                    return;

                MultiPanelPage oldPage = m_selectedPage;
                m_selectedPage = value;

                foreach (Control child in Controls)
                {
                    child.Visible = ReferenceEquals(child, m_selectedPage);
                }

                if (SelectionChange != null)
                    SelectionChange(null, new MultiPanelSelectionChangeEventArgs(oldPage, value));
            }
        }

        /// <summary>
        /// Repaint the panel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            using (SolidBrush br = new SolidBrush(BackColor))
                g.FillRectangle(br, ClientRectangle);
        }

        /// <summary>
        /// Overriden. Creates the underlying controls collection.
        /// </summary>
        /// <returns></returns>
        protected override ControlCollection CreateControlsInstance()
        {
            return new MultiPanelPagesCollection(this);
        }


        #region MultiPanelPagesCollection

        /// <summary>
        /// A collection of pages for the <see cref="MultiPanel"/> control.
        /// </summary>
        /// <remarks>
        /// Based on the work from Liron Levi on Code Project, under public domain. 
        /// See http://www.codeproject.com/KB/cs/multipanelcontrol.aspx
        /// </remarks>
        private sealed class MultiPanelPagesCollection : ControlCollection
        {
            private readonly MultiPanel m_owner;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="owner"></param>
            public MultiPanelPagesCollection(Control owner)
                : base(owner)
            {
                if (owner == null)
                    throw new ArgumentNullException("owner", "Tried to create a MultiPanelPagesCollection with a null owner.");

                m_owner = owner as MultiPanel;
                if (m_owner == null)
                    throw new ArgumentException("Tried to create a MultiPanelPagesCollection with a non-MultiPanel owner.",
                                                "owner");
            }

            /// <summary>
            /// Adds a page.
            /// </summary>
            /// <param name="value"></param>
            public override void Add(Control value)
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Tried to add a null value to the MultiPanelPagesCollection.");

                MultiPanelPage p = value as MultiPanelPage;
                if (p == null)
                    throw new ArgumentException("Tried to add a non-MultiPanelPage control to the MultiPanelPagesCollection",
                                                "value");

                p.SendToBack();
                base.Add(p);
            }

            /// <summary>
            /// Adds an array of pages
            /// </summary>
            /// <param name="controls"></param>
            public override void AddRange(Control[] controls)
            {
                foreach (MultiPanelPage p in controls)
                {
                    Add(p);
                }
            }

            /// <summary>
            /// Removes a page.
            /// </summary>
            /// <param name="value"></param>
            public override void Remove(Control value)
            {
                base.Remove(value);
            }

            /// <summary>
            /// Retrieves the index of the page with the given key.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public override int IndexOfKey(string key)
            {
                Control ctrl = base[key];
                return GetChildIndex(ctrl);
            }
        }

        #endregion
    }
}
