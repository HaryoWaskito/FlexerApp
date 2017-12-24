using System;

namespace FlexerApp.Models
{
    public class KeyboardMouseLogModel
    {
        public int KeyboardMouseLogModelId { get; set; }
        public int SessionId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }
        public long KeyStrokeCount { get; set; }
        public long MouseClickCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSuccessSendToServer { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RowStatus { get; set; }
    }
}
