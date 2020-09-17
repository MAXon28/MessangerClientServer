using System.Threading.Tasks;
using System.Windows.Input;
using ChatClient.Logic;
using ChatClient.Logic.UserLogic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ChatClient.ViewModel.Dialog
{
    public class LoginDialogViewModel : ViewModelBase
    {
        private string _newLogin;

        public LoginDialogViewModel()
        {
            CurrentLogin = UserContainer.Login;
        }

        public string CurrentLogin { get; set; }

        public string NewLogin
        {
            get => _newLogin;

            set
            {
                if (!value.Contains(" "))
                {
                    _newLogin = value;
                }
            }
        }

        public ICommand Confirmation
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (IsTrueData() && NewLogin != CurrentLogin)
                    {
                        await Task.Run(() => ServerWorker.NewInstance().UpdateLogin(NewLogin));
                    }
                });
            }
        }

        public bool IsTrueData()
        {
            if (string.IsNullOrEmpty(NewLogin) || NewLogin.Length < 4)
            {
                return false;
            }
            return true;
        }
    }
}