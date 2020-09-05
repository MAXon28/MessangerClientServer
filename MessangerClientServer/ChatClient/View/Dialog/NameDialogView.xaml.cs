using System.Windows;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class NameDialogView : Window
    {
        private NameDialogViewModel _nameDialogViewModel;

        public NameDialogView(NameDialogViewModel nameDialogViewModel)
        {
            InitializeComponent();
            _nameDialogViewModel = nameDialogViewModel;
            DataContext = _nameDialogViewModel;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (_nameDialogViewModel.IsTrueData())
            {
                Close();
            }
        }
    }
}