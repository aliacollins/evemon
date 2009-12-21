using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot]
    public class SerializableColor
    {
        private int a = 255;
        private int r = 0;
        private int g = 0;
        private int b = 0;

        public SerializableColor()
        {
        }

        public SerializableColor(int alpha, int red, int green, int blue)
        {
            a = alpha;
            r = red;
            g = green;
            b = blue;
        }

        public SerializableColor(Color c)
        {
            a = c.A;
            r = c.R;
            g = c.G;
            b = c.B;
        }

        [XmlAttribute]
        public int A
        {
            get { return a; }
            set { a = value; }
        }

        [XmlAttribute]
        public int R
        {
            get { return r; }
            set { r = value; }
        }

        [XmlAttribute]
        public int G
        {
            get { return g; }
            set { g = value; }
        }

        [XmlAttribute]
        public int B
        {
            get { return b; }
            set { b = value; }
        }

        public Color ToColor()
        {
            return Color.FromArgb(a, r, g, b);
        }
    }
}
