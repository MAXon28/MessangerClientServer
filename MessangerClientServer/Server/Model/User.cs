using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model
{
    [Table("Users")]
    class User
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Gender { get; set; }

        public string Name { get; set; }

        public DateTime? PastOnline { get; set; }

        public int ChatMessageId { get; set; }

        public virtual ICollection<ChatMessage> Messages { get; set; }

        public virtual ICollection<GameStatistic> GameStatistics { get; set; }

        public virtual ICollection<Settings> Settings { get; set; }

        public User()
        {
            Messages = new List<ChatMessage>();
            GameStatistics = new List<GameStatistic>();
            Settings = new List<Settings>();
        }
    }
}