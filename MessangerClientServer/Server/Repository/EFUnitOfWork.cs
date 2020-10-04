using System;
using Server.EF;
using Server.Interface;
using Server.Model;

namespace Server.Repository
{
    class EFUnitOfWork : IDisposable
    {
        private ChatDbContext _db;
        private UserRepository _userRepository;
        private ChatMessageRepository _chatMessageRepository;
        private GameRepository _gameRepository;
        private SettingsRepository _settingsRepository;

        public EFUnitOfWork()
        {
            _db = new ChatDbContext();
        }

        public IRepository<User> UsersRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_db);
                }

                return _userRepository;
            }
        }

        public IUser<User> Users
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_db);
                }

                return _userRepository;
            }
        }

        public IRepository<ChatMessage> MessagesRepository
        {
            get
            {
                if (_chatMessageRepository == null)
                {
                    _chatMessageRepository = new ChatMessageRepository(_db);
                }

                return _chatMessageRepository;
            }
        }

        public ICertainList<ChatMessage> CertainMessages
        {
            get
            {
                if (_chatMessageRepository == null)
                {
                    _chatMessageRepository = new ChatMessageRepository(_db);
                }

                return _chatMessageRepository;
            }
        }

        public IRepository<GameStatistic> GameRepository
        {
            get
            {
                if (_gameRepository == null)
                {
                    _gameRepository = new GameRepository(_db);
                }

                return _gameRepository;
            }
        }

        public IRepository<Settings> SettingsRepository
        {
            get
            {
                if (_settingsRepository == null)
                {
                    _settingsRepository = new SettingsRepository(_db);
                }

                return _settingsRepository;
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}