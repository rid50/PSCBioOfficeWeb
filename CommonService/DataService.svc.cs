﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DAO;
namespace CommonService
{
    public class DataService : IDataService
    {
        static public Dictionary<string, string> _settings = new Dictionary<string, string>();
        static DataService()
        {
            var client = new ConfigurationService();

            foreach (KeyValuePair<string, string> kvp in client.AppSettings())
            {
                _settings.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in client.ConnectionStrings())
            {
                _settings.Add(kvp.Key, kvp.Value);
            }
        }
        public void SendWSQImage(int id, ref byte[] buffer)
        {
            if (_settings["enroll"] == "db")
            {
                DataSource ds = null;

                if (_settings["dbProvider"] == "dedicatedServer")
                    ds = new DAO.Database(_settings);
                else if (_settings["dbProvider"] == "cloudServer")
                    ds = new DAO.CloudDatabase(_settings);
                else
                    throw new Exception("Wrong database provider settings");

                ds.SendImage(IMAGE_TYPE.wsq, Convert.ToInt32(id), ref buffer);
            }
        }
    }
}