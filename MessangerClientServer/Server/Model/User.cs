using System;
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
    }
}