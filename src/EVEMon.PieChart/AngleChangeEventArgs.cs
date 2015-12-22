using System;

namespace EVEMon.PieChart
{
    public class AngleChangeEventArgs : EventArgs
    {
        public AngleChangeEventArgs(float oldValue, float newValue)
        {
            OldAngle = oldValue;
            NewAngle = newValue;
        }

        public float NewAngle { get; private set; }

        public float OldAngle { get; private set; }
    }
}