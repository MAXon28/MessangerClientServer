﻿using System;

namespace ChatLibrary
{
    [Serializable]
    public class ChatMessageDTO
    {
        public int Id { get; set; }

        public string SenderName { get; set; }

        public string DateSend { get; set; }

        public string SendMessage { get; set; }
    }
}