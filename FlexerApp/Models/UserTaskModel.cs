using System;

namespace FlexerApp.Models
{
    public class UserTaskModel
    {
        public int UserTaskModelId { get; set; }
        public int SessionId { get; set; }
        public int TaskId { get; set; }
        public string TaskStatus { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RowStatus { get; set; }
    }
}
