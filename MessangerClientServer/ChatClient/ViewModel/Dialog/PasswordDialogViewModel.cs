using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.UserLogic;
using ChatClient.View.Dialog;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ChatClient.ViewModel.Dialog
{
    public class PasswordDialogViewModel : ViewModelBase, IHash
    {
        public PasswordDialogViewModel()
        {
            IsNotTrueCurrentPassword = false;
            IsNotTrueLength = false;
            IsDisagree = false;
        }

        public SecureString CurrentPassword { get; set; }

        public SecureString NewPassword { get; set; }

        public SecureString RepeatPassword { get; set; }

        public bool IsNotTrueCurrentPassword { get; set; }

        public bool IsNotTrueLength { get; set; }

        public bool IsDisagree { get; set; }

        public ICommand Confirmation
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    IsNotTrueCurrentPassword = false;
                    IsNotTrueLength = false;
                    IsDisagree = false;
                    if (IsTrueData() && NewPassword != CurrentPassword)
                    {
                        UserContainer.Password = NewPassword;
                        await Task.Run(() => ServerWorker.NewInstance().UpdatePassword(GetHash(new System.Net.NetworkCredential(string.Empty, NewPassword).Password)));
                    }
                });
            }
        }

        public bool IsTrueData()
        {
            if (new System.Net.NetworkCredential(string.Empty, CurrentPassword).Password != new System.Net.NetworkCredential(string.Empty, UserContainer.Password).Password)
            {
                IsNotTrueCurrentPassword = true;
                return false;
            }

            if (new System.Net.NetworkCredential(string.Empty, NewPassword).Password?.Length < 4 )
            {
                IsNotTrueLength = true;
                return false;
            }

            if (new System.Net.NetworkCredential(string.Empty, NewPassword).Password != new System.Net.NetworkCredential(string.Empty, RepeatPassword).Password)
            {
                IsDisagree = true;
                return false;
            }

            return true;
        }

        public string GetHash(string password)
        {
            var sha = new SHA1Managed();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}