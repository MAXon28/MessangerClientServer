using GalaSoft.MvvmLight;

namespace ChatClient.ViewModel.Dialog
{
    public class ErrorDialogViewModel : ViewModelBase
    {
        public ErrorDialogViewModel() { }

        public ErrorDialogViewModel(string error)
        {
            Error = error;
        }

        public string Error { get; set; }
    }
}