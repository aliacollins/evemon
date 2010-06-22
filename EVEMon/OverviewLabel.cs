using System;
using System.Drawing;
using System.Windows.Forms;

namespace EVEMon
{
    /// <summary>
    /// A panel that does not display a gray text when disabled (and we need to disable them so that the button does not always lose focus and its nice "I'm hovered" color)
    /// </summary>
    public sealed class OverviewLabel : Label
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewLabel"/> class.
        /// </summary>
        public OverviewLabel()
        {
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control can respond to user interaction.
        /// </summary>
        /// <value></value>
        /// <returns>true if the control can respond to user interaction; otherwise, false. The default is true.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public new Boolean Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Triggered when the label should be repainted.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            using (var foreground = new SolidBrush(this.ForeColor))
            {
                var format = new StringFormat();
                
                if (this.AutoEllipsis)
                    format.Trimming = StringTrimming.EllipsisCharacter;

                e.Graphics.DrawString(this.Text, this.Font, foreground, Padding.Left, Padding.Right, format);
            }
        }
    }
}
