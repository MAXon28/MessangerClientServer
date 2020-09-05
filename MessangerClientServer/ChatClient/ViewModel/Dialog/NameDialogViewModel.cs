using System.Threading.Tasks;
using System.Windows.Input;
using ChatClient.Logic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Dialog
{
    public class NameDialogViewModel : ViewModelBase
    {
        private string _newName;

        public NameDialogViewModel() { }

        public NameDialogViewModel(string name)
        {
            CurrentName = name;
        }

        public string CurrentName { get; set; }

        public string NewName
        {
            get => _newName;

            set
            {
                if (!value.Contains(" "))
                {
                    _newName = value;
                }
            }
        }

        public ICommand Confirmation
        {
            get
            {
                return new RelayCommand(async() =>
                {
                    if (IsTrueData() && NewName != CurrentName)
                    {
                        await Task.Run(() => ServerWorker.NewInstance().UpdateName(NewName));
                    }
                });
            }
        }

        public bool IsTrueData()
        {
            if (string.IsNullOrEmpty(NewName))
            {
                return false;
            }
            return true;
        }
    }
}