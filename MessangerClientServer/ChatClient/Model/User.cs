using System;
using SQLite;

namespace ChatClient.Model
{
    class User
    {
        [PrimaryKey, AutoIncrement, Unique]
        public Guid Id { get; set; }

        [MaxLength(28), NotNull]
        public string Login { get; set; }

        [MaxLength(19), NotNull]
        public string Password { get; set; }
    }
}