using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Dialog
{
    public class GameDialogViewModel : ViewModelBase
    {
        public GameDialogViewModel() {}

        public string UserResponse { get; private set; }

        public ICommand SayYes
        {
            get
            {
                return new RelayCommand(() =>
                {
                    UserResponse = "Да";
                });
            }
        }

        public ICommand SayNo
        {
            get
            {
                return new RelayCommand(() =>
                {
                    UserResponse = "Нет";
                });
            }
        }
    }
}