using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ChatClient.View
{
    public partial class RegistrationView : UserControl
    {
        public RegistrationView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            { ((dynamic)DataContext).Password = ((PasswordBox)sender).SecurePassword; }
        }

        private void PasswordRepeatBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            { ((dynamic)DataContext).PasswordRepeat = ((PasswordBox)sender).SecurePassword; }
        }
    }
}