using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Model;
using ChatClient.View;
using ChatClient.View.Dialog;
using ChatClient.ViewModel.Dialog;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel
{
    class MainViewModel : ViewModelBase, IHash
    {
        private ServerWorker _serverWorker = ServerWorker.NewInstance();
        private LogicDb _logicDb = new LogicDb();
        private MainWindow _viewWindow;
        private ServerConnection _serverConnection;

        public MainViewModel() { }

        public MainViewModel(MainWindow viewWindow)
        {
            _viewWindow = viewWindow;

            Password = new SecureString();

            if (_logicDb.GetCountSaveUsers() > 0)
            {
                (string, string) data = _logicDb.GetSaveData();
                Login = data.Item1;
                for (int i = 0; i < data.Item2.Length; i++)
                {
                    Password.AppendChar(data.Item2[i]);
                }
                _viewWindow.PasswordWrite(new System.Net.NetworkCredential(string.Empty, Password).Password);
            }

            IsNotAuthorization = false;
            IsCanClick = true;

            _serverConnection = ServerConnection.NewInstance();
        }

        public string Login { get; set; }

        public SecureString Password { get; set; }

        public bool IsRemember { get; set; }

        public bool IsNotAuthorization { get; set; }

        public bool IsCanClick { get; set; }

        public ICommand RestoreUser
        {
            get
            {
                return  new RelayCommand(() =>
                {
                    if (_logicDb.GetCountSaveUsers() > 1)
                    {
                        (string, string) data = _logicDb.GetSaveData();
                        Login = data.Item1;
                        Password.Clear();
                        for (int i = 0; i < data.Item2.Length; i++)
                        {
                            Password.AppendChar(data.Item2[i]);
                        }
                        _viewWindow.PasswordWrite(new System.Net.NetworkCredential(string.Empty, Password).Password);
                    }
                });
            }
        }

        public ICommand OpenChat => new RelayCommand(() =>
        {
            IsCanClick = false;
            bool isCanContinue = true;
            if (_serverConnection.Ip == null)
            {
                var serverDialogViewModel = new ServerDialogViewModel();
                ServerDialogView serverDialogView = new ServerDialogView(serverDialogViewModel);
                serverDialogView.ShowDialog();
                isCanContinue = serverDialogViewModel.IsConfirm;
            }

            if (isCanContinue)
            {
                ToServerAsync();
            }
            else
            {
                IsCanClick = true;
            }
        });

        public ICommand OpenRegistration
        {
            get
            {
                return new RelayCommand(() =>
                {
                    NewPage = new RegistrationView();
                    NewPage.DataContext = new RegistrationViewModel();
                });
            }
        }

        public ICommand OpenServerSettings
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ServerDialogView serverDialogView = new ServerDialogView(new ServerDialogViewModel());
                    serverDialogView.ShowDialog();
                });
            }
        }

        public FrameworkElement NewPage { get; set; }

        private async void ToServerAsync()
        {
            string resultCode = "";
            string name = "";
            string login = Login ?? "";
            string password = new System.Net.NetworkCredential(string.Empty, Password).Password;
            await Task.Run(() =>
            {
                try
                {
                    resultCode = _serverWorker.Authorization(login, GetHash(password), ref name);
                    if (resultCode == "28")
                    {
                        if (IsRemember)
                        {
                            Task.Run(() => _logicDb.AddNewUser(login, password));
                        }
                    }
                    else
                    {
                        IsNotAuthorization = true;
                    }
                }
                catch
                {
                    // ignore
                }
            });

            if (resultCode == "28")
            {
                MainChatPageViewModel mainChatPageViewModel = new MainChatPageViewModel(name);

                NewPage = new MainChatPageView();
                NewPage.DataContext = mainChatPageViewModel;
                mainChatPageViewModel.StartLoad();
            }
            else if (resultCode == "")
            {
                ErrorDialogView errorDialogView = new ErrorDialogView(new ErrorDialogViewModel("Ошибка подключения к серверу!"));
                errorDialogView.ShowDialog();
            }
            IsCanClick = true;
        }

        public string GetHash(string password)
        {
            var sha = new SHA1Managed();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}