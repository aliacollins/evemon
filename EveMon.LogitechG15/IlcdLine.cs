using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace EVEMon.LogitechG15
{
    public interface ILcdLine
    {
        // properties
        float Height { get; }

        // methods
        void Render(Graphics canvas, Graphics overlay, float offset);
    }
}
