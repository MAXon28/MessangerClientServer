using System.Windows;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class GenderDialogView : Window
    {
        public GenderDialogView(GenderDialogViewModel genderDialogViewModel)
        {
            InitializeComponent();
            DataContext = genderDialogViewModel;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}