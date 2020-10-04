using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using ChatClient.ViewModel.List;
using ChatLibrary;
using ChatLibrary.DataTransferObject;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Game
{
    class GamePlayViewModel : ViewModelBase, IViewModel
    {
        private ServerWorker _serverWorker;
        private List<List<string>> _playingField;
        private const string FIRST_PLAYER_TURN = "Ваш ход";
        private const string SECOND_PLAYER_TURN = "Ожидание хода ";
        private const string FIRST_PLAYER_WIN = "Вы победили!!!";
        private const string SECOND_PLAYER_WIN = "К сожалению, Вы проиграли...";
        private const string SECOND_PLAYER_GIVE_UP = " вышел. Вы победили!";
        private const string DRAW = "Ничья!";

        public GamePlayViewModel() { }

        public GamePlayViewModel(string name)
        {
            _serverWorker = ServerWorker.NewInstance();
            _playingField = new List<List<string>>();
            _playingField.Add(new List<string> { "", "", "" });
            _playingField.Add(new List<string> { "", "", "" });
            _playingField.Add(new List<string> { "", "", "" });
            Condition = "Visible";
            Name = name;
            IsVisibleGame = false;
            IsVisibleSpinner = true;
            IsVisibleCancel = false;

            OneOneSquare = "";
            OneTwoSquare = "";
            OneThreeSquare = "";
            TwoOneSquare = "";
            TwoTwoSquare = "";
            TwoThreeSquare = "";
            ThreeOneSquare = "";
            ThreeTwoSquare = "";
            ThreeThreeSquare = "";

            OneOneBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            OneTwoBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            OneThreeBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            TwoOneBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            TwoTwoBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            TwoThreeBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            ThreeOneBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            ThreeTwoBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
            ThreeThreeBack = (Brush)new BrushConverter().ConvertFromString("Transparent");
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public string OpponentName { get; set; }

        public bool IsVisibleGame { get; set; }

        public bool IsVisibleSpinner { get; set; }

        public bool IsVisibleCancel { get; private set; }

        public string OneOneSquare { get; set; }
        public string OneTwoSquare { get; set; }
        public string OneThreeSquare { get; set; }
        public string TwoOneSquare { get; set; }
        public string TwoTwoSquare { get; set; }
        public string TwoThreeSquare { get; set; }
        public string ThreeOneSquare { get; set; }
        public string ThreeTwoSquare { get; set; }
        public string ThreeThreeSquare { get; set; }

        public Brush OneOneBack { get; set; }
        public Brush OneTwoBack { get; set; }
        public Brush OneThreeBack { get; set; }
        public Brush TwoOneBack { get; set; }
        public Brush TwoTwoBack { get; set; }
        public Brush TwoThreeBack { get; set; }
        public Brush ThreeOneBack { get; set; }
        public Brush ThreeTwoBack { get; set; }
        public Brush ThreeThreeBack { get; set; }

        public string FirstGamerSymbol { get; private set; }

        public string SecondGamerSymbol { get; private set; }

        public string GameInformation { get; private set; }

        public bool IsEnable { get; set; }

        public string TextNotification { get; private set; }

        public bool IsVisibleNotification { get; set; }

        public ICommand ClickOneOne
        {
            get
            {
                return new RelayCommand(async() =>
                {
                    if (OneOneSquare == "")
                    {
                        IsEnable = false;
                        OneOneSquare = FirstGamerSymbol;
                        _playingField[0][0] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("00"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickOneTwo
        {
            get
            {
                return new RelayCommand(async() =>
                {
                    if (OneTwoSquare == "")
                    {
                        IsEnable = false;
                        OneTwoSquare = FirstGamerSymbol;
                        _playingField[0][1] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("01"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickOneThree
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (OneThreeSquare == "")
                    {
                        IsEnable = false;
                        OneThreeSquare = FirstGamerSymbol;
                        _playingField[0][2] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("02"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickTwoOne
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (TwoOneSquare == "")
                    {
                        IsEnable = false;
                        TwoOneSquare = FirstGamerSymbol;
                        _playingField[1][0] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("10"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickTwoTwo
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (TwoTwoSquare == "")
                    {
                        IsEnable = false;
                        TwoTwoSquare = FirstGamerSymbol;
                        _playingField[1][1] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("11"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickTwoThree
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (TwoThreeSquare == "")
                    {
                        IsEnable = false;
                        TwoThreeSquare = FirstGamerSymbol;
                        _playingField[1][2] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("12"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickThreeOne
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (ThreeOneSquare == "")
                    {
                        IsEnable = false;
                        ThreeOneSquare = FirstGamerSymbol;
                        _playingField[2][0] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("20"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickThreeTwo
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (ThreeTwoSquare == "")
                    {
                        IsEnable = false;
                        ThreeTwoSquare = FirstGamerSymbol;
                        _playingField[2][1] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("21"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand ClickThreeThree
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (ThreeThreeSquare == "")
                    {
                        IsEnable = false;
                        ThreeThreeSquare = FirstGamerSymbol;
                        _playingField[2][2] = FirstGamerSymbol;
                        await Task.Run(() => _serverWorker.SendPlayerSMove("22"));
                        GameInformation = SECOND_PLAYER_TURN + OpponentName;
                        CheckDraw();
                    }
                });
            }
        }

        public ICommand Cancel
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Condition = "Collapsed";
                });
            }
        }

        public void Notification(BinaryReader binaryReader)
        {
            string code = binaryReader.ReadString();
            if (code == "1-11")
            {
                IsVisibleGame = true;
                IsVisibleSpinner = false;
                OpponentName = binaryReader.ReadString();
                FirstGamerSymbol = binaryReader.ReadString();
                if (FirstGamerSymbol == "X")
                {
                    IsEnable = true;
                    SecondGamerSymbol = "0";
                    GameInformation = FIRST_PLAYER_TURN;
                }
                else
                {
                    IsEnable = false;
                    SecondGamerSymbol = "X";
                    GameInformation = SECOND_PLAYER_TURN + OpponentName;
                }
                TextNotification = $"Вы играете за {FirstGamerSymbol}";
                ShowNotificationAsync();
            }
            else if (code == "2-11")
            {
                IsVisibleSpinner = false;
            }
            else if (code == "11-0")
            {
                SetSymbolInPlayingField(SecondGamerSymbol, binaryReader.ReadString(), false);
                IsEnable = true;
                GameInformation = FIRST_PLAYER_TURN;
            }
            else if (code == "11-1")
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    GameInformation = FIRST_PLAYER_WIN;
                    string[] positions = binaryReader.ReadString().Split('-');
                    SetSymbolInPlayingField(FirstGamerSymbol, positions[0], true, "#00FF7F");
                    await Task.Delay(528);
                    SetSymbolInPlayingField(FirstGamerSymbol, positions[1], true, "#00FF7F");
                    await Task.Delay(528);
                    SetSymbolInPlayingField(FirstGamerSymbol, positions[2], true, "#00FF7F");
                    IsVisibleCancel = true;
                }, DispatcherPriority.Background);
            }
            else if (code == "11-2")
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    GameInformation = SECOND_PLAYER_WIN;
                    string[] positions = binaryReader.ReadString().Split('-');
                    SetSymbolInPlayingField(SecondGamerSymbol, positions[0], true, "#FA8072");
                    await Task.Delay(528);
                    SetSymbolInPlayingField(SecondGamerSymbol, positions[1], true, "#FA8072");
                    await Task.Delay(528);
                    SetSymbolInPlayingField(SecondGamerSymbol, positions[2], true, "#FA8072");
                    IsVisibleCancel = true;
                }, DispatcherPriority.Background);
            }
            else if (code == "11-3")
            {
                Application.Current.Dispatcher.Invoke( () =>
                {
                    GameInformation = DRAW;
                    SetSymbolInPlayingField(SecondGamerSymbol, binaryReader.ReadString(), false);
                    IsVisibleCancel = true;
                }, DispatcherPriority.Background);
            }
            else if (code == "11-4")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    GameInformation = OpponentName + SECOND_PLAYER_GIVE_UP;
                    IsVisibleCancel = true;
                }, DispatcherPriority.Background);
            }
            else if (code == "29")
            {
                NotificationTranslator.GetEnteringUserNotification(binaryReader.ReadString(), "Information");
            }
            else if (code == "30")
            {
                NotificationTranslator.PlaySoundNotificationAsync();
                NotificationTranslator.GetNewMessageNotification(binaryReader.ReadString());
            }
            else if (code == "40")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя изменено!", "Success");
                Name = binaryReader.ReadString();
            }
            else if (code == "41")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя не изменено! Пользователь с таким именем уже зарегестрирован!", "Error");
            }
            else if (code == "42")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя изменён!", "Success");
                UserContainer.Login = binaryReader.ReadString();
            }
            else if (code == "43")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя не изменён! Пользователь с таким логином уже зарегестрирован!", "Error");
            }
        }

        private void SetSymbolInPlayingField(string symbol, string position, bool isNewBackColor, string color = null)
        {
            switch (position)
            {
                case "00":
                    OneOneSquare = symbol;
                    _playingField[0][0] = symbol;
                    if (isNewBackColor)
                    {
                        OneOneBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "01":
                    OneTwoSquare = symbol;
                    _playingField[0][1] = symbol;
                    if (isNewBackColor)
                    {
                        OneTwoBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "02":
                    OneThreeSquare = symbol;
                    _playingField[0][2] = symbol;
                    if (isNewBackColor)
                    {
                        OneThreeBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "10":
                    TwoOneSquare = symbol;
                    _playingField[1][0] = symbol;
                    if (isNewBackColor)
                    {
                        TwoOneBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "11":
                    TwoTwoSquare = symbol;
                    _playingField[1][1] = symbol;
                    if (isNewBackColor)
                    {
                        TwoTwoBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "12":
                    TwoThreeSquare = symbol;
                    _playingField[1][2] = symbol;
                    if (isNewBackColor)
                    {
                        TwoThreeBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "20":
                    ThreeOneSquare = symbol;
                    _playingField[2][0] = symbol;
                    if (isNewBackColor)
                    {
                        ThreeOneBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "21":
                    ThreeTwoSquare = symbol;
                    _playingField[2][1] = symbol;
                    if (isNewBackColor)
                    {
                        ThreeTwoBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
                case "22":
                    ThreeThreeSquare = symbol;
                    _playingField[2][2] = symbol;
                    if (isNewBackColor)
                    {
                        ThreeThreeBack = (Brush)new BrushConverter().ConvertFromString(color);
                    }
                    break;
            }
        }

        private void CheckDraw()
        {
            if (!_playingField[0].Contains("") && !_playingField[1].Contains("") && !_playingField[2].Contains(""))
            {
                GameInformation = DRAW;
                IsVisibleCancel = true;
            }
        }

        /// <summary>
        /// Отображает уведомление о скопированном тексте
        /// </summary>
        private async void ShowNotificationAsync()
        {
            IsVisibleNotification = true;
            await Task.Delay(1928);
            IsVisibleNotification = false;
        }
    }
}