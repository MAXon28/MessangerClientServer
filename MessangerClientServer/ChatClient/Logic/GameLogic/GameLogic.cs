using System;
using System.Collections.Generic;
using ChatClient.ViewModel.Game;

namespace ChatClient.Logic.GameLogic
{
    class GameLogic
    {
        private GamePlayViewModel _user;
        private Computer _computer;
        private List<List<string>> _playingField;
        private string _currentSymbol;

        public GameLogic() { }

        public GameLogic(GamePlayViewModel user)
        {
            _user = user;
            _computer = new Computer(this);
            _playingField = new List<List<string>>();
            _playingField.Add(new List<string> { "null", "null", "null" });
            _playingField.Add(new List<string> { "null", "null", "null" });
            _playingField.Add(new List<string> { "null", "null", "null" });
            _currentSymbol = "X";
            WhoIsWho();
            _computer.Symbol = ComputerSymbol;
            if (ComputerSymbol == "X")
            {
                StartAsync();
            }
        }

        public string GamerSymbol { get; private set; }

        public string ComputerSymbol { get; private set; }

        public async void SetUserMoveAsync(int row, int column)
        {
            _playingField[row][column] = _currentSymbol;
            string typeOfGameOver = "";
            string reasonGameOver = "";
            if (IsGameOver(ref typeOfGameOver, ref reasonGameOver))
            {
                _user.SetGameOverWithComputer(typeOfGameOver, reasonGameOver);
                return;
            }
            _currentSymbol = ComputerSymbol;
            await _computer.ComputerResponse.Invoke(_playingField);
        }

        public void SetComputerMove(int row, int column)
        {
            _playingField[row][column] = _currentSymbol;
            string typeOfGameOver = "";
            string reasonGameOver = "";
            if (IsGameOver(ref typeOfGameOver, ref reasonGameOver))
            {
                if (typeOfGameOver == "lose")
                {
                    _user.SetGameOverWithComputer(typeOfGameOver, reasonGameOver);
                }
                else
                {
                    _user.SetGameOverWithComputer(typeOfGameOver, row.ToString() + column);
                }
                return;
            }
            _currentSymbol = GamerSymbol;
            _user.SetComputerMove(row, column);
        }

        private void WhoIsWho()
        {
            List<string> symbols = new List<string> { "X", "0" };
            int countRandom = 0;
            int countX = 0;
            int count0 = 0;
            var randomizer = new Random();
            while (countRandom < 3)
            {
                int index = randomizer.Next(0, 2);
                if (symbols[index] == "X")
                {
                    countX++;
                }
                else
                {
                    count0++;
                }

                countRandom++;
            }

            if (countX > 1)
            {
                GamerSymbol = symbols[0];
                ComputerSymbol = symbols[1];
            }

            if (count0 > 1)
            {
                GamerSymbol = symbols[1];
                ComputerSymbol = symbols[0];
            }
        }

        private async void StartAsync()
        {
            await _computer.ComputerResponse.Invoke(_playingField);
        }

        private bool IsGameOver(ref string typeOfGameOver, ref string reasonGameOver)
        {
            if (_playingField[0][0] == _currentSymbol && _playingField[0][1] == _currentSymbol && _playingField[0][2] == _currentSymbol)
            {
                reasonGameOver = "00-01-02";
            }
            else if (_playingField[1][0] == _currentSymbol && _playingField[1][1] == _currentSymbol && _playingField[1][2] == _currentSymbol)
            {
                reasonGameOver = "10-11-12";
            }
            else if (_playingField[2][0] == _currentSymbol && _playingField[2][1] == _currentSymbol && _playingField[2][2] == _currentSymbol)
            {
                reasonGameOver = "20-21-22";
            }
            else if (_playingField[0][0] == _currentSymbol && _playingField[1][0] == _currentSymbol && _playingField[2][0] == _currentSymbol)
            {
                reasonGameOver = "00-10-20";
            }
            else if (_playingField[0][1] == _currentSymbol && _playingField[1][1] == _currentSymbol && _playingField[2][1] == _currentSymbol)
            {
                reasonGameOver = "01-11-21";
            }
            else if (_playingField[0][2] == _currentSymbol && _playingField[1][2] == _currentSymbol && _playingField[2][2] == _currentSymbol)
            {
                reasonGameOver = "02-12-22";
            }
            else if (_playingField[0][2] == _currentSymbol && _playingField[1][1] == _currentSymbol && _playingField[2][0] == _currentSymbol)
            {
                reasonGameOver = "02-11-20";
            }
            else if (_playingField[0][0] == _currentSymbol && _playingField[1][1] == _currentSymbol && _playingField[2][2] == _currentSymbol)
            {
                reasonGameOver = "00-11-22";
            }

            if (reasonGameOver != "")
            {
                typeOfGameOver = GamerSymbol == _currentSymbol ? "win" : "lose";
                return true;
            }

            if (_playingField[0].IndexOf("null") == -1 && _playingField[1].IndexOf("null") == -1 && _playingField[2].IndexOf("null") == -1)
            {
                typeOfGameOver = "draw";
                return true;
            }

            return false;
        }
    }
}