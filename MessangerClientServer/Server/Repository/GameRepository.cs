using Server.Interface;
using Server.Model;
using System.Collections.Generic;
using Server.EF;

namespace Server.Repository
{
    class GameRepository : IRepository<GameStatistic>
    {
        private ChatDbContext _chatDbContext;

        public GameRepository(ChatDbContext db)
        {
            _chatDbContext = db;
        }

        public void Create(GameStatistic item)
        {
            _chatDbContext.GameStatistics.Add(item);
        }

        public IEnumerable<GameStatistic> GetAll()
        {
            return _chatDbContext.GameStatistics;
        }
    }
}