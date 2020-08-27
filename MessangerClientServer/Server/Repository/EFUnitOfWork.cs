using System;
using Server.EF;
using Server.Interface;
using Server.Model;
using Server.Repository;

namespace Server.Repository
{
    class EFUnitOfWork : IDisposable
    {
        private ChatDbContext _db;
        private UserRepository _userRepository;
        private ChatMessageRepository _chatMessageRepository;

        public EFUnitOfWork()
        {
            _db = new ChatDbContext();
        }

        public IRepository<User> Users
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

        public IRepository<ChatMessage> StandardMessages
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