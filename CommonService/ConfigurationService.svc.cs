using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Activation;

namespace CommonService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ConfigurationService : IConfigurationService
    {
        public IConfigurationService GetConfigurationManager()
        {
            return this;
        }
        
        public Dictionary<string, string> AppSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                settings.Add(key, ConfigurationManager.AppSettings[key]);
            }

            return settings; 
        }

        public Dictionary<string, string> ConnectionStrings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
            {
                settings.Add(cs.Name, cs.ConnectionString);
            }

            return settings;
        }
        
        public System.Configuration.ConnectionStringSettingsCollection ConnectionString {
            get { return ConfigurationManager.ConnectionStrings; } 
        }

        public string getAppSetting(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];

            // If we didn't find setting, try to load it from current dll's config file
            if (string.IsNullOrEmpty(setting))
            {
                var assemly = System.Reflection.Assembly.GetExecutingAssembly();
                var configuration = ConfigurationManager.OpenExeConfiguration(assemly.Location);
                var value = configuration.AppSettings.Settings[key];
                if (value != null)
                {
                    setting = value.Value;
                }
            }

            return setting;
        }

        public string getConnectionString(string name = "ConnectionString")
        {
            string connectionString = null;
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[name];
            if (connectionStringSettings != null)
                connectionString = connectionStringSettings.ConnectionString;

            // If we didn't find setting, try to load it from current dll's config file
            if (string.IsNullOrEmpty(connectionString))
            {
                var assemly = System.Reflection.Assembly.GetExecutingAssembly();
                var configuration = ConfigurationManager.OpenExeConfiguration(assemly.Location);
                var value = configuration.ConnectionStrings.ConnectionStrings[name];
                if (value != null)
                {
                    connectionString = value.ConnectionString;
                }
            }

            return connectionString;
        }
    }
}
