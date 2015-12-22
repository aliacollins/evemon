using System;

namespace EVEMon.Common.Controls
{
    public class ListViewDragEventArgs : EventArgs
    {
        internal ListViewDragEventArgs(int from, int count, int to)
        {
            MovingFrom = from;
            MovingCount = count;
            MovingTo = to;
        }

        public int MovingFrom { get; private set; }

        public int MovingCount { get; private set; }

        public int MovingTo { get; private set; }

        public bool Cancel { get; set; }
    }
}