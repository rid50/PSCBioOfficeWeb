using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationServer.Caching;
using System.Collections;
using System.Threading;

namespace AppFabricCacheService
{
    public class LookUp
    {
        private ArrayList   _fingerList;
        private byte[]      _template;
        private DataCache   _cache;
        private CancellationToken _ct;

        public LookUp(ArrayList fingerList, byte[] template, DataCache cache, CancellationToken ct)
        {
            _fingerList = fingerList;
            _template   = template;
            _cache      = cache;
            _ct         = ct;
        }

        public UInt32 run(String regionName)
        {
            byte[][] buffer = new byte[10][];

            foreach(KeyValuePair<string, object> item in _cache.GetObjectsInRegion(regionName)) {

                if (_ct.IsCancellationRequested)
                {
                    _ct.ThrowIfCancellationRequested();
                }
                
                if (item.Key == "123") {
                    buffer = item.Value as byte[][];
                    for(int i = 0; i < buffer.Length; i++) 
                    if ((buffer[i]).Length != 0)
                        return (UInt32)(buffer[i]).Length;
                }
            }
            
            return 0;
        }
    }
}