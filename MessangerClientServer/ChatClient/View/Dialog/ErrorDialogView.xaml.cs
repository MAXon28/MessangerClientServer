using System.Windows;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class ErrorDialogView : Window
    {
        public ErrorDialogView(ErrorDialogViewModel errorRegistrationDialogViewModel)
        {
            InitializeComponent();
            DataContext = errorRegistrationDialogViewModel;
        }
    }
}