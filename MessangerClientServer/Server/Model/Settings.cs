using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model
{
    [Table("Settings")]
    class Settings
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int TypeOfSoundAtNotificationNewMessage { get; set; }

        public string TypeOfNotification { get; set; }

        public User User { get; set; }
    }
}