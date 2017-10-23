using System;

namespace FlexerApp.Models
{
    public class ScreenshotLogModel
    {
        public string ScreenshotLogModelId { get; set; }
        public int SessionID { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }
        public byte[] Image { get; set; }
        public DateTime CaptureScreenDate { get; set; }
        public bool IsSuccessSendToServer { get; set; }
    }
}
