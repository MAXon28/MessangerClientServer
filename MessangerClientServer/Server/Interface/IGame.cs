using System;

namespace Server.Interface
{
    interface IGame<T> where T : class
    {
        void UpdateAllGameWithUsers(Guid userId);

        void UpdateWinWithUsers(Guid userId);

        void UpdateLoseWithUsers(Guid userId);

        void UpdateAllGamesWithComputer(Guid userId);

        void UpdateWinWithComputer(Guid userId);

        void UpdateLoseWithComputer(Guid userId);
    }
}