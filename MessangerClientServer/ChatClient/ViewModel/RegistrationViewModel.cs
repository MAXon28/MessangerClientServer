using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Model;
using ChatClient.View.Dialog;
using ChatClient.ViewModel.Dialog;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel
{
    class RegistrationViewModel : ViewModelBase, IHash
    {
        private ServerConnection _serverConnection;
        private string _login;
        private string _name;

        public RegistrationViewModel()
        {
            Condition = "Visible";
            Login = "";
            Name = "";
            StandardColor();
            _serverConnection = ServerConnection.NewInstance();
            IndexGender = -1;
        }

        public string Condition { get; set; }

        public string Login
        {
            get => _login;
            set
            {
                if (!value.Contains(" "))
                {
                    _login = value;
                }
            }
        }

        public SecureString Password { private get; set; }

        public SecureString PasswordRepeat { private get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (!value.Contains(" "))
                {
                    _name = value;
                }
            }
        }

        public int IndexGender { get; set; }

        public Brush LoginBack { get; set; }

        public Brush PasswordBack { get; set; }

        public Brush GenderBack { get; set; }

        public Brush NameBack { get; set; }

        public int CountPasswordRepeat => PasswordRepeat?.Length ?? 0;

        public ICommand Registration
        {
            get
            {
                return new RelayCommand(() =>
                {
                    bool isReady = IsRightFilling() && IsReady();
                    if (isReady)
                    {
                        Condition = "Collapsed";
                    }
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

        public ICommand Back
        {
            get
            {
                return new RelayCommand(() => { Condition = "Collapsed"; });
            }
        }

        private bool IsRightFilling()
        {
            string rowsErrors = "";
            if (Login?.Length < 4)
            {
                rowsErrors += "1";
            }

            if (new System.Net.NetworkCredential(string.Empty, Password).Password?.Length < 4)
            {
                rowsErrors += "2";
            }

            if (IndexGender == -1)
            {
                rowsErrors += "3";
            }

            if (Name?.Length < 3)
            {
                rowsErrors += "4";
            }

            if (rowsErrors != "")
            {
                RewriteBackgroundAsync(rowsErrors);
                return false;
            }

            if (new System.Net.NetworkCredential(string.Empty, Password).Password != new System.Net.NetworkCredential(string.Empty, PasswordRepeat).Password)
            {
                ErrorDialogView errorDialogView = new ErrorDialogView(new ErrorDialogViewModel("Пароли не совпадают!"));
                errorDialogView.ShowDialog();
                return false;
            }

            return true;
        }

        private bool IsReady()
        {
            bool isReady = true;

            string gender = IndexGender == 0 ? "man" : "woman";

            try
            {
                var serverWorker = ServerWorker.NewInstance(); ;
                int resultCode = serverWorker.Registration(Login, GetHash(new System.Net.NetworkCredential(string.Empty, Password).Password), gender, Name);

                if (resultCode == 33)
                {
                    ErrorDialogView errorDialogView = new ErrorDialogView(new ErrorDialogViewModel("Это имя или этот логин уже заняты!"));
                    errorDialogView.ShowDialog();
                    isReady = false;
                }

                return isReady;
            }
            catch
            {
                ErrorDialogView errorDialogView = new ErrorDialogView(new ErrorDialogViewModel("Ошибка подключения к серверу!"));
                errorDialogView.ShowDialog();
                return false;
            }
        }

        public string GetHash(string password)
        {
            var sha = new SHA1Managed();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }

        private async void RewriteBackgroundAsync(string errors)
        {
            if (errors.Contains("1"))
            {
                LoginBack = (Brush)new BrushConverter().ConvertFromString("#DC143C");
            }

            if (errors.Contains("2"))
            {
                PasswordBack = (Brush)new BrushConverter().ConvertFromString("#DC143C");
            }

            if (errors.Contains("3"))
            {
                GenderBack = (Brush)new BrushConverter().ConvertFromString("#DC143C");
            }

            if (errors.Contains("4"))
            {
                NameBack = (Brush)new BrushConverter().ConvertFromString("#DC143C");
            }
            await Task.Delay(328);
            StandardColor();
        }

        private void StandardColor()
        {
            LoginBack = (Brush)new BrushConverter().ConvertFromString("#373737");
            PasswordBack = (Brush)new BrushConverter().ConvertFromString("#373737");
            GenderBack = (Brush)new BrushConverter().ConvertFromString("#373737");
            NameBack = (Brush)new BrushConverter().ConvertFromString("#373737");
        }
    }
}