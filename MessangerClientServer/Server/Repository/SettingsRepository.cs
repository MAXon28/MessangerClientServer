using System.Collections.Generic;
using Server.EF;
using Server.Interface;
using Server.Model;

namespace Server.Repository
{
    class SettingsRepository : IRepository<Settings>
    {
        private ChatDbContext _chatDbContext;

        public SettingsRepository(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }

        public IEnumerable<Settings> GetAll()
        {
            return _chatDbContext.Settings;
        }

        public void Create(Settings item)
        {
            _chatDbContext.Settings.Add(item);
        }
    }
}