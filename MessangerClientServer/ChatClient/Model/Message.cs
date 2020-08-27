using System;
using GalaSoft.MvvmLight;

namespace ChatClient.Model
{
    [Serializable]
    public class Message : ViewModelBase
    {
        public int Id { get; set; }

        public string SenderName { get; set; }

        public string DateSend { get; set; }

        public string SendMessage { get; set; }

        public bool IsItMe { get; set; }
    }
}