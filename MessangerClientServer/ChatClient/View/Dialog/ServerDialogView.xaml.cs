using System.Windows;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class ServerDialogView : Window
    {
        private ServerDialogViewModel _serverDialogViewModel;

        public ServerDialogView(ServerDialogViewModel serverDialogViewModel)
        {
            InitializeComponent();
            _serverDialogViewModel = serverDialogViewModel;
            DataContext = _serverDialogViewModel;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (_serverDialogViewModel.IsTrueData())
            {
                this.Close();
            }
        }
    }
}