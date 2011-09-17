using System;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class UpdateAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="min">Minimum length of time between updates.</param>
        /// <param name="cacheStyle">Cache style.</param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod min, CacheStyle cacheStyle)
        {
            CreateUpdateAttribute(defaultPeriod, min, UpdatePeriod.Week, cacheStyle);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="min">Minimum length of time between updates.</param>
        /// <param name="max">Maximum length of time between updates.</param>
        /// <param name="cacheStyle">Cache style.</param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod min, UpdatePeriod max, CacheStyle cacheStyle)
        {
            CreateUpdateAttribute(defaultPeriod, min, max, cacheStyle);
        }

        /// <summary>
        /// Constructor helper method.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="min">Minimum length of time between updates.</param>
        /// <param name="max">Maximum length of time between updates.</param>
        /// <param name="cacheStyle">Cache style.</param>
        private void CreateUpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod min, UpdatePeriod max, CacheStyle cacheStyle)
        {
            DefaultPeriod = defaultPeriod;
            Minimum = min;
            Maximum = max;
            CacheStyle = cacheStyle;
        }

        #endregion


        #region Public Properties

        public UpdatePeriod DefaultPeriod { get; private set; }

        public UpdatePeriod Minimum { get; private set; }

        public UpdatePeriod Maximum { get; private set; }

        public CacheStyle CacheStyle { get; private set; }

        #endregion
    }
}