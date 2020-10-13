using System.Windows;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class GameDialogView : Window
    {
        public GameDialogView(GameDialogViewModel gameDialogViewModel)
        {
            InitializeComponent();
            DataContext = gameDialogViewModel;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}