using System;
using System.Windows.Input;
using ChatClient.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Dialog
{
    public class ServerDialogViewModel : ViewModelBase
    {
        private ServerConnection _serverConnection;
        private string _firstIp;
        private string _secondIp;
        private string _thirdIp;
        private string _fourthIp;
        private string _port;

        public ServerDialogViewModel()
        {
            _serverConnection = ServerConnection.NewInstance();
            IsConfirm = false;
            if (_serverConnection.Ip != null)
            {
                string[] partsIp = _serverConnection.Ip.Split(new[] {'.'});
                FirstIp = partsIp[0];
                SecondIp = partsIp[1];
                ThirdIp = partsIp[2];
                FourthIp = partsIp[3];
                Port = _serverConnection.Port.ToString();
            }
        }

        public string FirstIp
        {
            get { return _firstIp; }
            set
            {
                if (IsCorrectIp(value))
                {
                    _firstIp = value;
                }
            }
        }

        public string SecondIp
        {
            get { return _secondIp; }
            set
            {
                if (IsCorrectIp(value))
                {
                    _secondIp = value;
                }
            }
        }

        public string ThirdIp
        {
            get { return _thirdIp; }
            set
            {
                if (IsCorrectIp(value))
                {
                    _thirdIp = value;
                }
            }
        }

        public string FourthIp
        {
            get { return _fourthIp; }
            set
            {
                if (IsCorrectIp(value))
                {
                    _fourthIp = value;
                }
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                if (IsCorrectPort(value))
                {
                    _port = value;
                }
            }
        }

        public bool IsConfirm { get; set; }

        public ICommand Confirmation
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (IsTrueData())
                    {
                        _serverConnection.Ip = FirstIp + "." + SecondIp + "." + ThirdIp + "." + FourthIp;
                        _serverConnection.Port = Convert.ToInt32(Port);
                        _serverConnection.Serialize();
                        IsConfirm = true;
                    }
                });
            }
        }

        private bool IsCorrectIp(string value)
        {
            if (value.Length == 0)
            {
                return true;
            }
            if (int.TryParse(value, out var number))
            {
                if (number > 0 && number <= 255)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCorrectPort(string value)
        {
            return value.Length == 0 || int.TryParse(value, out var number);
        }

        public bool IsTrueData()
        {
            return FirstIp != null && SecondIp != null && ThirdIp != null && FourthIp != null && Port != null &&
                   FirstIp != "" && SecondIp != "" && ThirdIp != "" && FourthIp != "" && Port != "";
        }
    }
}