using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatLibrary.DataTransferObject;
using Server.Interface;
using Server.Model;
using Server.Repository;

namespace Server.BusinessLogic
{
    class GameService : IService
    {
        private EFUnitOfWork _efUnitOfWork;

        private const int GAME_WITH_USER = 1;
        private const int GAME_WITH_COMPUTER = 2;

        private const int WIN = 1;
        private const int LOSE = 2;

        public GameService()
        {
            _efUnitOfWork = new EFUnitOfWork();
        }

        public GameService(EFUnitOfWork efUnitOfWork)
        {
            _efUnitOfWork = efUnitOfWork;
        }

        public void AddGamer(Guid userId)
        {
            GameStatistic gameStatistic = new GameStatistic
            {
                UserId = userId,
                AllGameWithUsers = 0,
                WinWithUsers = 0,
                LoseWithUsers = 0,
                AllGameWithComputer = 0,
                LoseWithComputer = 0,
                WinWithComputer = 0
            };
            _efUnitOfWork.GameRepository.Create(gameStatistic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeOfGame"> 1 - игра с другим игроком, 2 - игра с компьютером </param>
        /// <param name="typeOfGameOver"> 1 - победа, 2 - проигрыш, 3 - ничья </param>
        public void UpdateStatistic(Guid userId, int typeOfGame, int typeOfGameOver)
        {
            switch (typeOfGame)
            {
                case GAME_WITH_USER:
                     _efUnitOfWork.GameStatistic.UpdateAllGameWithUsers(userId);
                    if (typeOfGameOver == WIN)
                    {
                        _efUnitOfWork.GameStatistic.UpdateWinWithUsers(userId);
                    }
                    else if (typeOfGameOver == LOSE)
                    {
                        _efUnitOfWork.GameStatistic.UpdateLoseWithUsers(userId);
                    }
                    break;
                case GAME_WITH_COMPUTER:
                    _efUnitOfWork.GameStatistic.UpdateAllGamesWithComputer(userId);
                    if (typeOfGameOver == WIN)
                    {
                        _efUnitOfWork.GameStatistic.UpdateWinWithComputer(userId);
                    }
                    else if (typeOfGameOver == LOSE)
                    {
                        _efUnitOfWork.GameStatistic.UpdateLoseWithComputer(userId);
                    }
                    break;
            }
        }

        public async Task<(List<RatingDTO>, List<RatingDTO>, List<RatingDTO>)> GetGameRatingsAsync()
        {
            var listRatingOverall = await Task.Run(GetRatingOverall);
            var listRatingWithUsers = await Task.Run(GetRatingWithUsers);
            var listRatingWithComputer = await Task.Run(GetRatingWithComputer);
            return (listRatingOverall, listRatingWithUsers, listRatingWithComputer);
        }

        private List<RatingDTO> GetRatingOverall()
        {
            var listDb = _efUnitOfWork.GameRepository.GetAll();
            var resultList = new List<RatingDTO>();
            foreach (var data in listDb)
            {
                resultList.Add(new RatingDTO
                {
                    Name = data.User.Name,
                    CountAllGame = data.AllGameWithUsers + data.AllGameWithComputer,
                    CountWin = data.WinWithUsers +data.WinWithComputer,
                    CountDraw = data.AllGameWithUsers + data.AllGameWithComputer - (data.WinWithUsers + data.WinWithComputer + data.LoseWithUsers + data.LoseWithComputer),
                    CountLose = data.LoseWithUsers + data.LoseWithComputer
                });
            }
            return GetSortList(resultList);
        }

        private List<RatingDTO> GetRatingWithUsers()
        {
            var listDb = _efUnitOfWork.GameRepository.GetAll();
            var resultList = new List<RatingDTO>();
            foreach (var data in listDb)
            {
                resultList.Add(new RatingDTO
                {
                    Name = data.User.Name,
                    CountAllGame = data.AllGameWithUsers,
                    CountWin = data.WinWithUsers,
                    CountDraw = data.AllGameWithUsers - (data.WinWithUsers + data.LoseWithUsers),
                    CountLose = data.LoseWithUsers
                });
            }
            return GetSortList(resultList);
        }

        private List<RatingDTO> GetRatingWithComputer()
        {
            var listDb = _efUnitOfWork.GameRepository.GetAll();
            var resultList = new List<RatingDTO>();
            foreach (var data in listDb)
            {
                resultList.Add(new RatingDTO
                {
                    Name = data.User.Name,
                    CountAllGame =  data.AllGameWithComputer,
                    CountWin = data.WinWithComputer,
                    CountDraw = data.AllGameWithComputer - (data.WinWithComputer + data.LoseWithComputer),
                    CountLose = data.LoseWithComputer
                });
            }
            return GetSortList(resultList);
        }

        private List<RatingDTO> GetSortList(List<RatingDTO> list)
        {
            List<RatingDTO> sortList = (from element in list
                orderby ((float)element.CountWin / element.CountAllGame * 100) descending, element.CountWin descending, element.CountDraw descending, element.CountLose
                                        select element).ToList();
            for (int i = 0; i < sortList.Count; i++)
            {
                sortList[i].Number = i + 1;
            }
            return sortList;
        }

        public EFUnitOfWork GetUnitOfWork()
        {
            return _efUnitOfWork;
        }

        public async void SaveAsync()
        {
            await Task.Run(_efUnitOfWork.Save);
        }
    }
}