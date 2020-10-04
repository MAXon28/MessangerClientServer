using System.Windows;
using System.Windows.Input;
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

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}