using System.Windows;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class LoginDialogView : Window
    {
        private LoginDialogViewModel _loginDialogViewModel;

        public LoginDialogView(LoginDialogViewModel loginDialogViewModel)
        {
            InitializeComponent();
            _loginDialogViewModel = loginDialogViewModel;
            DataContext = _loginDialogViewModel;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (_loginDialogViewModel.IsTrueData())
            {
                Close();
            }
        }
    }
}