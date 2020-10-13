using System.Collections.Generic;
using System.Linq;
using ChatLibrary.DataTransferObject;

namespace Server.BusinessLogic
{
    class SessionData
    {
        private static SessionData _sessionData;

        private SessionData()
        {
            DataMessages = new List<ChatMessageDTO>();
        }

        public static SessionData NewInstance()
        {
            if (_sessionData == null)
            {
                _sessionData = new SessionData();
            }
            return _sessionData;
        }

        public List<ChatMessageDTO> DataMessages { get; set; }

        public void SortList()
        {
            DataMessages = (from data in _sessionData.DataMessages
                orderby data.Id
                select data).ToList();
        }

        public void AddStart(ChatMessageDTO chatMessage)
        {
            DataMessages.Add(DataMessages[DataMessages.Count - 1]);
            for (int i = DataMessages.Count - 2; i >= 0; i--)
            {
                if (i == 0)
                {
                    DataMessages[i] = chatMessage;
                }
                else
                {
                    DataMessages[i] = DataMessages[i - 1];
                }
            }
        }

        public int IndexOf(ChatMessageDTO chatMessage)
        {
            foreach (var message in DataMessages)
            {
                if (message.Id == chatMessage.Id)
                {
                    return DataMessages.IndexOf(message);
                }
            }
            return -1;
        }
    }
}