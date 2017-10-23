using System;

namespace FlexerApp.Models
{
    public class LoginModel
    {
        public string LoginModelId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string LocationType { get; set; }
        public string IPAddress { get; set; }
        public string City { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }
        public int sessionID { get; set; }
        public string loginToken { get; set; }
        public float GMTDiff { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
