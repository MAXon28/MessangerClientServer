using System.Data.Entity;
using Server.Model;

namespace Server.EF
{
    class ChatDbContext : DbContext
    {
        public ChatDbContext() : base ("DBConnection") { }

        public DbSet<User> Users { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Settings> Settings { get; set; }
    }
}