using System;

namespace FlexerApp.Models
{
    public class LoginModel
    {
        public int LoginModelId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string LocationType { get; set; }
        public string IPAddress { get; set; }
        public string City { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public int SessionId { get; set; }
        public string LoginToken { get; set; }
        public decimal GMTDiff { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RowStatus { get; set; }
    }
}
