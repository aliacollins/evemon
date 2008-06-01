using System.Collections.Generic;
namespace EVEMon.Common
{
    /// <summary>
    /// A Singleton class to hold state information for the EVE API. Instances of this class should be accessed
    /// using Singleton.Instance&lt;APIState&gt;().
    /// </summary>
    [Singleton]
    public sealed class APIState
    {
        private bool _debugMode = false;
        private Settings _settings = Settings.GetInstance();

        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// Returns true if custom configurations can be defined and used for API calls. The default is false.
        /// </summary>
        public bool DebugMode
        {
            get { return _debugMode; }
            set { _debugMode = value; }
        }

        /// <summary>
        /// Returns the currently selected APIConfiguration. If DebugMode is false, returns the default configuration.
        /// </summary>
        public APIConfiguration CurrentConfiguration
        {
            get
            {
                if (_debugMode)
                {
                    foreach (APIConfiguration configuration in _settings.APIConfigurations)
                    {
                        if (configuration.Name == _settings.CustomAPIConfiguration)
                            return configuration;
                    }
                }
                return APIConfiguration.DefaultConfiguration;
            }
        }
    }
}
