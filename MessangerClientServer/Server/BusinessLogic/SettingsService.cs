using System;
using System.Linq;
using System.Threading.Tasks;
using Server.Interface;
using Server.Model;
using Server.Repository;

namespace Server.BusinessLogic
{
    class SettingsService : IService
    {
        private EFUnitOfWork _efUnitOfWork;

        public SettingsService()
        {
            _efUnitOfWork = new EFUnitOfWork();
        }

        public SettingsService(EFUnitOfWork efUnitOfWork)
        {
            _efUnitOfWork = efUnitOfWork;
        }

        public void AddNewUserSettings(Guid userId)
        {
            Settings settings = new Settings
            {
                TypeOfSoundAtNotificationNewMessage = 0,
                TypeOfNotification = "All on",
                UserId = userId
            };
            _efUnitOfWork.SettingsRepository.Create(settings);

            SaveAsync();
        }

        public (int, string) GetSettingsByUserId(Guid userId)
        {
            Settings settings = (from s in _efUnitOfWork.SettingsRepository.GetAll()
                where s.UserId == userId
                select s).ToList()[0];
            return (settings.TypeOfSoundAtNotificationNewMessage, settings.TypeOfNotification);
        }

        public EFUnitOfWork GetUnitOfWork()
        {
            return  _efUnitOfWork;
        }

        public async void SaveAsync()
        {
            await Task.Run(_efUnitOfWork.Save);
        }
    }
}