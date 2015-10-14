namespace ImageHandler
{
    using Microsoft.ApplicationServer.Caching;
    using System.Collections.Generic;
    using System;

    public class CacheUtil
    {
        private static DataCacheFactory _factory = null;
        private static DataCache _cache = null;

        public static DataCache GetCache()
        {
            if (_cache != null)
                return _cache;

            //Define Array for 1 Cache Host
            List<DataCacheServerEndpoint> servers = new List<DataCacheServerEndpoint>(1);

            //Specify Cache Host Details 
            //  Parameter 1 = host name
            //  Parameter 2 = cache port number
            servers.Add(new DataCacheServerEndpoint(System.Environment.MachineName, 22233));

            //Create cache configuration
            DataCacheFactoryConfiguration configuration = new DataCacheFactoryConfiguration();

            //Set the cache host(s)
            configuration.Servers = servers;

            //Set default properties for local cache (local cache disabled)
            configuration.LocalCacheProperties = new DataCacheLocalCacheProperties();
            //configuration.ChannelOpenTimeout = System.TimeSpan.FromMinutes(2);
            configuration.RequestTimeout = TimeSpan.FromSeconds(1);
            configuration.ChannelOpenTimeout = TimeSpan.FromSeconds(45);
            configuration.TransportProperties.ReceiveTimeout = TimeSpan.FromSeconds(45);
            configuration.TransportProperties.ChannelInitializationTimeout = TimeSpan.FromSeconds(10);
            configuration.MaxConnectionsToServer = 1;
            //Disable tracing to avoid informational/verbose messages on the web page
            DataCacheClientLogManager.ChangeLogLevel(System.Diagnostics.TraceLevel.Off);

            //Pass configuration settings to cacheFactory constructor
            _factory = new DataCacheFactory(configuration);

            //Get reference to named cache called "default"
            _cache = _factory.GetCache("default");

            return _cache;
        }
    }
}