using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Network;

namespace Server.Game
{
    class GameLogic
    {
        private List<List<string>> _playingField;
        private string _currentSymbol;
        private int _moveCounter;

        public GameLogic()
        {
            _playingField = new List<List<string>>();
            _playingField.Add(new List<string> { "null", "null", "null" });
            _playingField.Add(new List<string> { "null", "null", "null" });
            _playingField.Add(new List<string> { "null", "null", "null" });
            _currentSymbol = "X";
            _moveCounter = 0;
        }

        public Client FirstGamer { get; set; }

        public Client SecondGamer { get; set; }

        public string FirstGamerSymbol { get; set; }

        public string SecondGamerSymbol { get; set; }

        public void Move(int row, int column)
        {
            _playingField[row][column] = _currentSymbol;
            if (_moveCounter >= 2)
            {
                string typeOfGameOver = "";
                string reasonGameOver = "";
                bool isGameOver = IsGameOver(ref typeOfGameOver, ref reasonGameOver);
                if (isGameOver)
                {
                    if (typeOfGameOver == "First gamer win")
                    {
                        FirstGamer.GameOver("11-1", reasonGameOver);
                        SecondGamer.GameOver("11-2", reasonGameOver);
                    }
                    else if (typeOfGameOver == "Second gamer win")
                    {
                        FirstGamer.GameOver("11-2", reasonGameOver);
                        SecondGamer.GameOver("11-1", reasonGameOver);
                    }
                    else
                    {
                        if (_currentSymbol == FirstGamerSymbol)
                        {
                            SecondGamer.GameOver("11-3", row.ToString() + column);
                        }
                        else
                        {
                            FirstGamer.GameOver("11-3", row.ToString() + column);
                        }
                    }
                    return;
                }
            }

            if (FirstGamerSymbol == _currentSymbol)
            {
                SecondGamer.GameOpponentMove(row + column.ToString());
            }
            else
            {
                FirstGamer.GameOpponentMove(row + column.ToString());
            }
            _currentSymbol = _currentSymbol == "X" ? "0" : "X";
            if (_currentSymbol == "X")
            {
                _moveCounter++;
            }
        }

        public void EarlyGameOver(Client client)
        {
            if (FirstGamer == client)
            {
                SecondGamer.EarlyVictory();
            }
            else
            {
                FirstGamer.EarlyVictory();
            }
        }

        private bool IsGameOver(ref string typeOfGameOver, ref string reasonGameOver)
        {
            if (_playingField[0].IndexOf("null") == -1 && _playingField[1].IndexOf("null") == -1 && _playingField[2].IndexOf("null") == -1)
            {
                typeOfGameOver = "draw";
                return true;
            }
            
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
                typeOfGameOver = FirstGamerSymbol == _currentSymbol ? "First gamer win" : "Second gamer win";
                return true;
            }

            return false;
        }
    }
}