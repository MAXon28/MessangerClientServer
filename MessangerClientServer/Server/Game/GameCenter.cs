using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Network;

namespace Server.Game
{
    static class GameCenter
    {
        private static Client _userWhoWait;
        private static bool _isWait;

        public static async Task<string> ConnectAsync(Client client)
        {
            if (_userWhoWait == null)
            {
                _userWhoWait = client;
                _isWait = true;
                return await Task.Run(Result);
            }

            (string, string) whoIsWho = WhoIsWho();
            var game = new GameLogic { FirstGamer = _userWhoWait, SecondGamer = client, FirstGamerSymbol = whoIsWho.Item1, SecondGamerSymbol = whoIsWho.Item2 };

            _userWhoWait.SetGameLogic(game);
            client.SetGameLogic(game);
            _userWhoWait = null;

            return "Have gamer";
        }

        public static void OutTheGame(Client client)
        {
            if (_userWhoWait == client)
            {
                _isWait = false;
                _userWhoWait = null;
            }
        }

        private static string Result()
        {
            int startTime = DateTime.Now.Second != 0 ? DateTime.Now.Second - 1 : 59;
            while (DateTime.Now.Second != startTime && _userWhoWait != null && _isWait) ;

            if (!_isWait)
            {
                return "";
            }

            if (_userWhoWait == null)
            {
                return "Have gamer";
            }

            _userWhoWait = null;
            return "Have not gamer";
        }

        private static (string, string) WhoIsWho()
        {
            List<string> symbols = new List<string> { "X", "0" };
            int index = new Random().Next(0, 2);
            var firstGamerSymbol = symbols[index];
            symbols.RemoveAt(index);
            var secondGamerSymbol = symbols[0];
            return (firstGamerSymbol, secondGamerSymbol);
        }
    }
}