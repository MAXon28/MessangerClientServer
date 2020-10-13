using System;
using System.Collections.Generic;
using System.Linq;
using Server.EF;
using Server.Interface;
using Server.Model;

namespace Server.Repository
{
    class SettingsRepository : IRepository<Settings>, ISettings<Settings>
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

        public void UpdateTypeOfSoundAtNotificationNewMessage(Guid userId, int typeOfSound)
        {
            var settings = (from data in _chatDbContext.Settings
                where data.UserId == userId
                select data).ToList()[0];
            settings.TypeOfSoundAtNotificationNewMessage = typeOfSound;
            _chatDbContext.SaveChanges();
        }

        public void UpdateTypeOfNotification(Guid userId, string typeOfNotification)
        {
            var settings = (from data in _chatDbContext.Settings
                where data.UserId == userId
                select data).ToList()[0];
            settings.TypeOfNotification = typeOfNotification;
            _chatDbContext.SaveChanges();
        }
    }
}