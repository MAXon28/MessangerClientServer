using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Dialog
{
    public class GameDialogViewModel : ViewModelBase
    {
        public GameDialogViewModel()
        {
            DialogText = "Если Вы выйдете из игры, Вам засчитают поражение. Вы действительно хотите покинуть игру?";
        }

        public GameDialogViewModel(string text)
        {
            DialogText = text;
        }

        public string UserResponse { get; private set; }

        public string DialogText { get; }

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