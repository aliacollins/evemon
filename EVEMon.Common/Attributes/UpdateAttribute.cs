using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Attributes
{
    public sealed class UpdateAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultPeriod"></param>
        /// <param name="min"></param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod min)
        {
            this.DefaultPeriod = defaultPeriod;
            this.Maximum = UpdatePeriod.Week;
            this.Minimum = min;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultPeriod"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod min, UpdatePeriod max)
        {
            this.DefaultPeriod = defaultPeriod;
            this.Minimum = min;
            this.Maximum = max;
        }

        public UpdatePeriod DefaultPeriod
        {
            get;
            set;
        }

        public UpdatePeriod Minimum
        {
            get;
            set;
        }

        public UpdatePeriod Maximum
        {
            get;
            set;
        }

    }
}
