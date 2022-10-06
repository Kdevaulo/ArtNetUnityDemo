using System;
using System.Net;

namespace Models
{
    [Serializable]
    public class CustomIPAddress
    {
        public string CurrentIP;

        public IPAddress IPAddress;

        public string IPLabel;
    }
}