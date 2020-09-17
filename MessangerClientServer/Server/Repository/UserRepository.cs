using System;
using System.Collections.Generic;
using System.Linq;
using Server.EF;
using Server.Interface;
using Server.Model;

namespace Server.Repository
{
    class UserRepository : IRepository<User>, IUser<User>
    {
        private ChatDbContext _chatDbContext;

        public UserRepository(ChatDbContext db)
        {
            _chatDbContext = db;
        }

        public IEnumerable<User> GetAll()
        {
            if (_chatDbContext.Users.Any())
            {
                return _chatDbContext.Users;
            }
            else
            {
                return null;
            }
            //return _chatDbContext.Users.Any() ? _chatDbContext.Users : null;
        }

        public void Create(User user)
        {
            _chatDbContext.Users.Add(user);
        }

        public void UpdateName(Guid userId, string newName)
        {
            User user = _chatDbContext.Users.Find(userId);
            user.Name = newName;
            _chatDbContext.SaveChanges();
        }

        public void UpdateGender(Guid userId, string newGender)
        {
            User user = _chatDbContext.Users.Find(userId);
            user.Gender = newGender;
            _chatDbContext.SaveChanges();
        }

        public void UpdateLogin(Guid userId, string newLogin)
        {
            User user = _chatDbContext.Users.Find(userId);
            user.Login = newLogin;
            _chatDbContext.SaveChanges();
        }

        public void UpdatePassword(Guid userId, string newPassword)
        {
            User user = _chatDbContext.Users.Find(userId);
            user.Password = newPassword;
            _chatDbContext.SaveChanges();
        }

        public void UpdatePastOnline(Guid userId, DateTime newPastOnline)
        {
            User user = _chatDbContext.Users.Find(userId);
            user.PastOnline = newPastOnline;
            _chatDbContext.SaveChanges();
        }
    }
}