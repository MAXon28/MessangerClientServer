using System.Windows;
using System.Windows.Controls;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class PasswordDialogView : Window
    {
        private PasswordDialogViewModel _passwordDialogViewModel;

        public PasswordDialogView(PasswordDialogViewModel passwordDialogViewModel)
        {
            InitializeComponent();
            _passwordDialogViewModel = passwordDialogViewModel;
            DataContext = _passwordDialogViewModel;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (_passwordDialogViewModel.IsTrueData())
            {
                Close();
            }
        }

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            { ((dynamic)DataContext).CurrentPassword = ((PasswordBox)sender).SecurePassword; }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            { ((dynamic)DataContext).NewPassword = ((PasswordBox)sender).SecurePassword; }
        }

        private void RepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            { ((dynamic)DataContext).RepeatPassword = ((PasswordBox)sender).SecurePassword; }
        }
    }
}