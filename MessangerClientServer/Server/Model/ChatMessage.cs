using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model
{
    [Table("ChatMessages")]
    class ChatMessage
    {
        public int Id { get; set; }

        public int IndexMessage { get; set; }

        public Guid UserId { get; set; }

        public DateTime TimeSendMessage { get; set; }

        public string Message { get; set; }

        public virtual User User { get; set; }
    }
}