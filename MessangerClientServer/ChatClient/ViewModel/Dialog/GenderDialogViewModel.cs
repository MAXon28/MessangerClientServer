using System.Threading.Tasks;
using System.Windows.Input;
using ChatClient.Logic;
using ChatClient.Logic.UserLogic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Dialog
{
    public class GenderDialogViewModel : ViewModelBase
    {
        private int _startTypeOfGender;

        public GenderDialogViewModel()
        {
            IndexGender = UserContainer.Gender == "woman" ? 1 : 0;
            _startTypeOfGender = IndexGender;
        }

        public int IndexGender { get; set; }

        public ICommand Confirmation
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (IndexGender != _startTypeOfGender)
                    {
                        UserContainer.Gender = IndexGender == 1 ? "woman" : "man";
                        await Task.Run(() => ServerWorker.NewInstance().UpdateGender(UserContainer.Gender));
                    }
                });
            }
        }
    }
}