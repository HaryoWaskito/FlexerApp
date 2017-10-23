using System;
using System.Net;
using System.Xml;

namespace FlexerApp.Controllers
{
    public class Locator
    {
        public string LocationType;
        public string PrivateIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        public string PublicIP;
        public string CountryCode;
        public string CountryName;
        public string RegionCode;
        public string RegionName;
        public string City;
        public string ZipCode;
        public string TimeZone;
        public long GMT;
        public float Latitude;
        public float Longitude;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Locator"/> class.
        /// </summary>
        public Locator()
        {
            GetPublicIP();
            GetTimeZone();
            GetGMT();            
        }

        /// <summary>
        /// Gets the public ip.
        /// </summary>
        /// <returns></returns>
        private void GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            PublicIP = a4;
        }

        /// <summary>
        /// Gets the time zone.
        /// </summary>
        /// <returns></returns>
        private void GetTimeZone()
        {
            TimeZone = string.Empty;
        }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        private void GetGMT()
        {
            GMT = 0;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        private void GetLocation()
        {

        }
    }
}
