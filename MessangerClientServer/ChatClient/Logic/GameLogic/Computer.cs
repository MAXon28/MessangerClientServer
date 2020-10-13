using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatClient.Logic.GameLogic
{
    delegate Task DelegateComputerResponse(List<List<string>> playingField);

    class Computer
    {
        private GameLogic _gameLogic;
        private int _countMove;

        public Computer(GameLogic gameLogic)
        {
            _gameLogic = gameLogic;
            _countMove = 0;
            if (new Random().Next(1, 3) == 1)
            {
                ComputerResponse = SetUpdateDifficultGameMoveAsync;
            }
            else
            {
                ComputerResponse = SetUpdateEasyGameMoveAsync;
            }
        }

        public string Symbol { get; set; }

        public DelegateComputerResponse ComputerResponse { get; }

        private async Task SetUpdateEasyGameMoveAsync(List<List<string>> playingField)
        {
            await Task.Delay(1000);
            if (_countMove >= 2)
            {
                string positionDefense = CheckWinOrLose(playingField, Symbol == "0" ? "X" : "0");
                if (positionDefense != "null")
                {
                    _gameLogic.SetComputerMove(positionDefense[0] - '0', positionDefense[1] - '0');
                    return;
                }
                string positionAttack = CheckWinOrLose(playingField, Symbol);
                if (positionAttack != "null")
                {
                    _gameLogic.SetComputerMove(positionAttack[0] - '0', positionAttack[1] - '0');
                }
                else
                {
                    GetRandomizeValue(playingField);
                }
            }
            else
            {
                GetRandomizeValue(playingField);
            }
            _countMove++;
        }

        private async Task SetUpdateDifficultGameMoveAsync(List<List<string>> playingField)
        {
            await Task.Delay(1000);
            if (_countMove == 0 && playingField[1][1] == "null")
            {
                _gameLogic.SetComputerMove(1, 1);
                _countMove++;
                return;
            }
            if (_countMove == 0)
            {
                if (playingField[0][0] == "null")
                {
                    _gameLogic.SetComputerMove(0, 0);
                }
                else if (playingField[0][2] == "null")
                {
                    _gameLogic.SetComputerMove(0, 2);
                }
                else if (playingField[2][0] == "null")
                {
                    _gameLogic.SetComputerMove(2, 0);
                }
                else if (playingField[2][2] == "null")
                {
                    _gameLogic.SetComputerMove(2, 2);
                }
                else
                {
                    GetRandomizeValue(playingField);
                }
                _countMove++;
                return;
            }

            if (_countMove == 1)
            {
                if (Symbol == "0")
                {
                    string positionDefense = CheckWinOrLose(playingField, "X");
                    if (positionDefense != "null")
                    {
                        _gameLogic.SetComputerMove(positionDefense[0] - '0', positionDefense[1] - '0');
                        _countMove++;
                        return;
                    }
                }

                if (playingField[0][0] == "null")
                {
                    _gameLogic.SetComputerMove(0, 0);
                }
                else if (playingField[0][2] == "null")
                {
                    _gameLogic.SetComputerMove(0, 2);
                }
                else if (playingField[2][0] == "null")
                {
                    _gameLogic.SetComputerMove(2, 0);
                }
                else if (playingField[2][2] == "null")
                {
                    _gameLogic.SetComputerMove(2, 2);
                }
                else
                {
                    GetRandomizeValue(playingField);
                }
                _countMove++;
            }
            else if (_countMove == 2)
            {
                string positionAttack = CheckWinOrLose(playingField, Symbol);
                if (positionAttack != "null")
                {
                    _gameLogic.SetComputerMove(positionAttack[0] - '0', positionAttack[1] - '0');
                    return;
                }

                string positionDefense = CheckWinOrLose(playingField, Symbol == "0" ? "X" : "0");
                if (positionDefense != "null")
                {
                    _gameLogic.SetComputerMove(positionDefense[0] - '0', positionDefense[1] - '0');
                    _countMove++;
                    return;
                }

                if (Symbol == "0")
                {
                    string positionPossibleVictory = CheckPossibleVictory(playingField);
                    if (positionPossibleVictory != "null")
                    {
                        _gameLogic.SetComputerMove(positionPossibleVictory[0] - '0', positionPossibleVictory[1] - '0');
                        _countMove++;
                        return;
                    }
                }

                if (playingField[0][0] == "null")
                {
                    _gameLogic.SetComputerMove(0, 0);
                }
                else if (playingField[0][2] == "null")
                {
                    _gameLogic.SetComputerMove(0, 2);
                }
                else if (playingField[2][0] == "null")
                {
                    _gameLogic.SetComputerMove(2, 0);
                }
                else if (playingField[2][2] == "null")
                {
                    _gameLogic.SetComputerMove(2, 2);
                }
                else
                {
                    GetRandomizeValue(playingField);
                }
                _countMove++;
            }
            else
            {
                string positionAttack = CheckWinOrLose(playingField, Symbol);
                if (positionAttack != "null")
                {
                    _gameLogic.SetComputerMove(positionAttack[0] - '0', positionAttack[1] - '0');
                    return;
                }

                string positionDefense = CheckWinOrLose(playingField, Symbol == "0" ? "X" : "0");
                if (positionDefense != "null")
                {
                    _gameLogic.SetComputerMove(positionDefense[0] - '0', positionDefense[1] - '0');
                    _countMove++;
                    return;
                }

                string positionPossibleVictory = CheckPossibleVictory(playingField);
                if (positionPossibleVictory != "null")
                {
                    _gameLogic.SetComputerMove(positionPossibleVictory[0] - '0', positionPossibleVictory[1] - '0');
                }
                else
                {
                    GetRandomizeValue(playingField);
                }
                _countMove++;
            }
        }

        private string CheckWinOrLose(List<List<string>> field, string symbol)
        {
            if (field[0][0] == "null")
            {
                if (field[0][1] == symbol && field[0][2] == symbol)
                {
                    return "00";
                }

                if (field[1][0] == symbol && field[2][0] == symbol)
                {
                    return "00";
                }

                if (field[1][1] == symbol && field[2][2] == symbol)
                {
                    return "00";
                }
            }

            if (field[0][1] == "null")
            {
                if (field[0][0] == symbol && field[0][2] == symbol)
                {
                    return "01";
                }

                if (field[1][1] == symbol && field[2][1] == symbol)
                {
                    return "01";
                }
            }

            if (field[0][2] == "null")
            {
                if (field[0][0] == symbol && field[0][1] == symbol)
                {
                    return "02";
                }

                if (field[1][2] == symbol && field[2][2] == symbol)
                {
                    return "02";
                }

                if (field[1][1] == symbol && field[2][0] == symbol)
                {
                    return "02";
                }
            }

            if (field[1][0] == "null")
            {
                if (field[0][0] == symbol && field[2][0] == symbol)
                {
                    return "10";
                }

                if (field[1][1] == symbol && field[1][2] == symbol)
                {
                    return "10";
                }
            }

            if (field[1][1] == "null")
            {
                if (field[0][0] == symbol && field[2][2] == symbol)
                {
                    return "11";
                }

                if (field[0][2] == symbol && field[2][0] == symbol)
                {
                    return "11";
                }

                if (field[0][1] == symbol && field[2][1] == symbol)
                {
                    return "11";
                }

                if (field[1][0] == symbol && field[1][2] == symbol)
                {
                    return "11";
                }
            }

            if (field[1][2] == "null")
            {
                if (field[1][0] == symbol && field[1][1] == symbol)
                {
                    return "12";
                }

                if (field[0][2] == symbol && field[2][2] == symbol)
                {
                    return "12";
                }
            }

            if (field[2][0] == "null")
            {
                if (field[0][0] == symbol && field[1][0] == symbol)
                {
                    return "20";
                }

                if (field[0][2] == symbol && field[1][1] == symbol)
                {
                    return "20";
                }

                if (field[2][1] == symbol && field[2][2] == symbol)
                {
                    return "20";
                }
            }

            if (field[2][1] == "null")
            {
                if (field[2][0] == symbol && field[2][2] == symbol)
                {
                    return "21";
                }

                if (field[0][1] == symbol && field[1][1] == symbol)
                {
                    return "21";
                }
            }

            if (field[2][2] == "null")
            {
                if (field[0][0] == symbol && field[1][1] == symbol)
                {
                    return "22";
                }

                if (field[2][0] == symbol && field[2][1] == symbol)
                {
                    return "22";
                }

                if (field[0][2] == symbol && field[1][2] == symbol)
                {
                    return "22";
                }
            }

            return "null";
        }

        private string CheckPossibleVictory(List<List<string>> field)
        {
            if (field[0][0] == Symbol)
            {
                if (field[0][1] == "null" && field[0][2] == "null")
                {
                    return "01";
                }

                if (field[1][0] == "null" && field[2][0] == "null")
                {
                    return "10";
                }

                if (field[1][1] == "null" && field[2][2] == "null")
                {
                    return "11";
                }
            }

            if (field[0][1] == Symbol)
            {
                if (field[0][0] == "null" && field[0][2] == "null")
                {
                    return "00";
                }

                if (field[1][1] == "null" && field[2][1] == "null")
                {
                    return "11";
                }
            }

            if (field[0][2] == Symbol)
            {
                if (field[0][0] == "null" && field[0][1] == "null")
                {
                    return "00";
                }

                if (field[1][2] == "null" && field[2][2] == "null")
                {
                    return "12";
                }

                if (field[1][1] == "null" && field[2][0] == "null")
                {
                    return "11";
                }
            }

            if (field[1][0] == Symbol)
            {
                if (field[0][0] == "null" && field[2][0] == "null")
                {
                    return "00";
                }

                if (field[1][1] == "null" && field[1][2] == "null")
                {
                    return "11";
                }
            }

            if (field[1][1] == Symbol)
            {
                if (field[0][0] == "null" && field[2][2] == "null")
                {
                    return "00";
                }

                if (field[0][2] == "null" && field[2][0] == "null")
                {
                    return "02";
                }

                if (field[0][1] == "null" && field[2][1] == "null")
                {
                    return "01";
                }

                if (field[1][0] == "null" && field[1][2] == "null")
                {
                    return "10";
                }
            }

            if (field[1][2] == Symbol)
            {
                if (field[1][0] == "null" && field[1][1] == "null")
                {
                    return "10";
                }

                if (field[0][2] == "null" && field[2][2] == "null")
                {
                    return "02";
                }
            }

            if (field[2][0] == Symbol)
            {
                if (field[0][0] == "null" && field[1][0] == "null")
                {
                    return "00";
                }

                if (field[0][2] == "null" && field[1][1] == "null")
                {
                    return "02";
                }

                if (field[2][1] == "null" && field[2][2] == "null")
                {
                    return "21";
                }
            }

            if (field[2][1] == Symbol)
            {
                if (field[2][0] == "null" && field[2][2] == "null")
                {
                    return "20";
                }

                if (field[0][1] == "null" && field[1][1] == "null")
                {
                    return "01";
                }
            }

            if (field[2][2] == Symbol)
            {
                if (field[0][0] == "null" && field[1][1] == "null")
                {
                    return "00";
                }

                if (field[2][0] == "null" && field[2][1] == "null")
                {
                    return "20";
                }

                if (field[0][2] == "null" && field[1][2] == "null")
                {
                    return "02";
                }
            }

            return "null";
        }

        private void GetRandomizeValue(List<List<string>> list)
        {
            int row = -1;
            int column = -1;
            var randomize = new Random();
            while (row == -1 || column == -1)
            {
                int indexRow = randomize.Next(0, 3);
                int indexColumn = randomize.Next(0, 3);
                if (list[indexRow][indexColumn] == "null")
                {
                    row = indexRow;
                    column = indexColumn;
                }
            }
            _gameLogic.SetComputerMove(row, column);
        }
    }
}