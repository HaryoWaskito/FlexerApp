using System;

namespace FlexerApp.Models
{
    public class ScreenshotLogModel
    {
        public int ScreenshotLogModelId { get; set; }
        public int SessionId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }
        public byte[] Image { get; set; }
        public DateTime CaptureScreenDate { get; set; }
        public bool IsSuccessSendToServer { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RowStatus { get; set; }
    }
}
