using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Configuration;

namespace WcfSiteConfigurationService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ConfigurationService : IConfigurationService
    {
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
