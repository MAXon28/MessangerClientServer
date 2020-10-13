using System;
using Server.Interface;
using Server.Model;
using System.Collections.Generic;
using System.Linq;
using Server.EF;

namespace Server.Repository
{
    class GameRepository : IRepository<GameStatistic>, IGame<GameStatistic>
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

        public void UpdateAllGameWithUsers(Guid userId)
        {
            var statistic = (from game in _chatDbContext.GameStatistics
                where game.UserId == userId
                select game).ToList()[0];
            statistic.AllGameWithUsers++;
            _chatDbContext.SaveChanges();
        }

        public void UpdateWinWithUsers(Guid userId)
        {
            var statistic = (from game in _chatDbContext.GameStatistics
                where game.UserId == userId
                select game).ToList()[0];
            statistic.WinWithUsers++;
            _chatDbContext.SaveChanges();
        }

        public void UpdateLoseWithUsers(Guid userId)
        {
            var statistic = (from game in _chatDbContext.GameStatistics
                where game.UserId == userId
                select game).ToList()[0];
            statistic.LoseWithUsers++;
            _chatDbContext.SaveChanges();
        }

        public void UpdateAllGamesWithComputer(Guid userId)
        {
            var statistic = (from game in _chatDbContext.GameStatistics
                where game.UserId == userId
                select game).ToList()[0];
            statistic.AllGameWithComputer++;
            _chatDbContext.SaveChanges();
        }

        public void UpdateWinWithComputer(Guid userId)
        {
            var statistic = (from game in _chatDbContext.GameStatistics
                where game.UserId == userId
                select game).ToList()[0];
            statistic.WinWithComputer++;
            _chatDbContext.SaveChanges();
        }

        public void UpdateLoseWithComputer(Guid userId)
        {
            var statistic = (from game in _chatDbContext.GameStatistics
                where game.UserId == userId
                select game).ToList()[0];
            statistic.LoseWithComputer++;
            _chatDbContext.SaveChanges();
        }
    }
}