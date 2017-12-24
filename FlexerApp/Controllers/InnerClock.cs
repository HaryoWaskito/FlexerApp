using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FlexerApp.Controllers
{
    public class InnerClock
    {
        public DateTime NISTDate;
        public DateTime NetworkDate;

        public InnerClock()
        {
            GetNetworkTime();
            GetNISTDate(false);
        }

        /// <summary>
        /// Gets the network time.
        /// </summary>
        /// <returns></returns>
        private void GetNetworkTime()
        {
            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var actualTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            NetworkDate = actualTime;
        }

        /// <summary>
        /// Swaps the endianness.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        /// <summary>
        /// Gets the nist date.
        /// </summary>
        /// <param name="convertToLocalTime">if set to <c>true</c> [convert to local time].</param>
        private void GetNISTDate(bool convertToLocalTime)
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            DateTime date = DateTime.Today;
            string serverResponse = string.Empty;

            // Represents the list of NIST servers
            string[] servers = new string[] {
                         "64.90.182.55",
                         "206.246.118.250",
                         "207.200.81.113",
                         "128.138.188.172",
                         "64.113.32.5",
                         "64.147.116.229",
                         "64.125.78.85",
                         "128.138.188.172"
                          };

            // Try each server in random order to avoid blocked requests due to too frequent request
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    // Open a StreamReader to a random time server
                    StreamReader reader = new StreamReader(new System.Net.Sockets.TcpClient(servers[ran.Next(0, servers.Length)], 13).GetStream());
                    serverResponse = reader.ReadToEnd();
                    reader.Close();

                    // Check to see that the signiture is there
                    if (serverResponse.Length > 47 && serverResponse.Substring(38, 9).Equals("UTC(NIST)"))
                    {
                        // Parse the date
                        int jd = int.Parse(serverResponse.Substring(1, 5));
                        int yr = int.Parse(serverResponse.Substring(7, 2));
                        int mo = int.Parse(serverResponse.Substring(10, 2));
                        int dy = int.Parse(serverResponse.Substring(13, 2));
                        int hr = int.Parse(serverResponse.Substring(16, 2));
                        int mm = int.Parse(serverResponse.Substring(19, 2));
                        int sc = int.Parse(serverResponse.Substring(22, 2));

                        if (jd > 51544)
                            yr += 2000;
                        else
                            yr += 1999;

                        date = new DateTime(yr, mo, dy, hr, mm, sc);

                        // Convert it to the current timezone if desired
                        if (convertToLocalTime)
                            date = date.ToLocalTime();

                        // Exit the loop
                        break;
                    }

                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }

            NISTDate = date;
        }
    }
}
