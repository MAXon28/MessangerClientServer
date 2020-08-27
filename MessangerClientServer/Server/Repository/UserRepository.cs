using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.EF;
using Server.Interface;
using Server.Model;

namespace Server.Repository
{
    class UserRepository : IRepository<User>
    {
        private ChatDbContext _chatDbContext;

        public UserRepository(ChatDbContext db)
        {
            _chatDbContext = db;
        }

        public IEnumerable<User> GetAll()
        { 
            return _chatDbContext.Users.Any() ? _chatDbContext.Users : null;
        }

        public void Create(User user)
        {
            _chatDbContext.Users.Add(user);
        }
    }
}