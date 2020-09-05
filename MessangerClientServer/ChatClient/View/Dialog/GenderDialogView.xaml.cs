using System.Windows;
using ChatClient.ViewModel.Dialog;

namespace ChatClient.View.Dialog
{
    public partial class GenderDialogView : Window
    {
        private GenderDialogViewModel _genderDialogViewModel;

        public GenderDialogView(GenderDialogViewModel genderDialogViewModel)
        {
            InitializeComponent();
            _genderDialogViewModel = genderDialogViewModel;
            DataContext = _genderDialogViewModel;
        }
    }
}