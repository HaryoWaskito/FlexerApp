using System;
using System.Net;
using System.Xml;

namespace FlexerApp.Controllers
{
    public class Locator
    {
        public string LocationType;
        public string PrivateIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        public string PublicIP = string.Empty;
        public string CountryCode = string.Empty;
        public string CountryName = string.Empty;
        public string RegionCode = string.Empty;
        public string RegionName = string.Empty;
        public string City = string.Empty;
        public string ZipCode = string.Empty;
        public string TimeZone = string.Empty;
        public decimal GMT = 0.00M;
        public decimal Latitude = 0.00M;
        public decimal Longitude = 0.00M;

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
            //string url = "http://checkip.dyndns.org";
            //WebRequest req = WebRequest.Create(url);            
            //WebResponse resp = req.GetResponse();
            //System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            //string response = sr.ReadToEnd().Trim();
            //string[] a = response.Split(':');
            //string a2 = a[1].Substring(1);
            //string[] a3 = a2.Split('<');
            //string a4 = a3[0];
            //PublicIP = a4;            
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
            GMT = 8.00M;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        private void GetLocation()
        {

        }
    }
}
