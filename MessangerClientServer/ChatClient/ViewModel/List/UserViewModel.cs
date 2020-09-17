using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace ChatClient.ViewModel.List
{
    class UserViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public Brush CircleColor { get; set; }

        /// <summary>
        /// Когда был онлайн
        /// </summary>
        public string PastDateOnline { get; set; }
    }
}