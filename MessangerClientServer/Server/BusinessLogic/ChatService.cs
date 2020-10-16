using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatLibrary.DataTransferObject;
using Server.Model;
using Server.Repository;
using Server.Interface;

namespace Server.BusinessLogic
{
    class ChatService : IService
    {
        private EFUnitOfWork _efUnitOfWork;
        private SessionData _sessionData;

        public ChatService()
        {
            _efUnitOfWork = new EFUnitOfWork();
            _sessionData = SessionData.NewInstance();
        }

        public ChatService(EFUnitOfWork efUnitOfWork)
        {
            _efUnitOfWork = efUnitOfWork;
            _sessionData = SessionData.NewInstance();
        }

        public ChatMessageDTO AddMessage(Guid userId, DateTime timeSendMessage, string message, string name)
        {
            var chatMessage = new ChatMessage
            {
                IndexMessage = _efUnitOfWork.MessagesRepository.GetAll().Count() + 1,
                UserId = userId,
                TimeSendMessage = timeSendMessage,
                Message = message
            };
            _efUnitOfWork.MessagesRepository.Create(chatMessage);

            var chatMessageDTO = new ChatMessageDTO
            {
                Id = chatMessage.IndexMessage,
                SenderName = name,
                DateSend = timeSendMessage.ToString(),
                SendMessage = message
            };
            _sessionData.DataMessages.Add(chatMessageDTO);

            SaveAsync();

            return chatMessageDTO;
        }

        public List<ChatMessageDTO> GetCurrentReadMessages(int messageId)
        {
            var chatMessages = (from message in _efUnitOfWork.MessagesRepository.GetAll()
                where message.Id <= messageId
                select message).ToList();
            chatMessages = chatMessages.Count > 100 ? chatMessages.Skip(chatMessages.Count - 100).ToList() : chatMessages;
            List<ChatMessageDTO> chatMessagesDTO = new List<ChatMessageDTO>();
            foreach (var message in chatMessages)
            {
                chatMessagesDTO.Add(GetReadyData(message));
            }
            return chatMessagesDTO;
        }

        /// <summary>
        /// Метод, который отвечает за считывание сообщений из базы данных и из временного контейнера (создан, чтобы постоянно не обращаться к базе данных)
        /// </summary>
        /// <param name="chatMessageId"> Номер ID, который указывает, каким было предыдущее отправленное сообщение </param>
        /// <returns> Возвращает список первых ста или меньше сообщений  </returns>
        public List<ChatMessageDTO> GetChatMessages(int chatMessageId = -1)
        {
            var list = new List<ChatMessageDTO>();
            if (_sessionData.DataMessages.Count == 0)
            {
                List<ChatMessage> fullList = _efUnitOfWork.MessagesRepository.GetAll().ToList();
                int lastIndex = fullList.Count >= 100 ? fullList.Count - 101 : -1;
                for (int i = fullList.Count - 1; i > lastIndex; i--)
                {
                    _sessionData.DataMessages.Add(GetReadyData(fullList[i]));
                }
                _sessionData.SortList();
                return _sessionData.DataMessages;
            }

            if (_sessionData.DataMessages.Count >= 100 && chatMessageId == -1)
            {
                for (int i = _sessionData.DataMessages.Count - 100; i < _sessionData.DataMessages.Count; i++)
                {
                    list.Add(_sessionData.DataMessages[i]);
                }
                return list;
            }

            if (_sessionData.DataMessages.Count < 100 && chatMessageId == -1)
            {
                return _sessionData.DataMessages;
            }

            ChatMessageDTO message = (from data in _sessionData.DataMessages
                                      where data.Id == chatMessageId
                                      select data).ToList()[0];
            int index = _sessionData.IndexOf(message);
            if (index >= 100)
            {
                int startIndex = index - 100;
                for (int i = startIndex; i < index; i++)
                {
                    list.Add(_sessionData.DataMessages[i]);
                }
                return list;
            }

            var listData = _efUnitOfWork.CertainMessages.GetMessagesBeforeId(_sessionData.DataMessages[0].Id).ToList();
            int needCountData = listData.Count > 100 - index ? listData.Count - 100 - index : 0;
            for (int i = listData.Count - 1; i >= needCountData; i--)
            {
                _sessionData.AddStart(GetReadyData(listData[i]));
            }
            for (int i = 0; i < _sessionData.DataMessages.IndexOf(message); i++)
            {
                list.Add(_sessionData.DataMessages[i]);
            }
            return list;
        }

        public (List<ChatMessageDTO>, int) GetNewMessagesAndAllCountNotReadMessages(List<ChatMessageDTO> chatMessages)
        {
            var list = new List<ChatMessageDTO>();
            if (_sessionData.DataMessages.Count == 0)
            {
                _sessionData.DataMessages = chatMessages;
                int count = chatMessages.Count;
                List <ChatMessage> fullList = _efUnitOfWork.CertainMessages.GetMessagesAfterId(chatMessages[chatMessages.Count - 1].Id).ToList();
                foreach (var message in fullList)
                {
                    _sessionData.DataMessages.Add(GetReadyData(message));
                }
                list = _sessionData.DataMessages.Skip(count).Take(200).ToList();
                return (list, _sessionData.DataMessages.Count - list.Count - count);
            }

            int index = _sessionData.IndexOf(chatMessages[0]);
            if (index > -1)
            {
                int startIndex = index + chatMessages.Count;
                for (int i = startIndex; i < _sessionData.DataMessages.Count; i++)
                {
                    list.Add(_sessionData.DataMessages[i]);
                }
                return (list.Take(200).ToList(), list.Count - list.Take(200).ToList().Count);
            }

            if (chatMessages[chatMessages.Count - 1].Id >= _sessionData.DataMessages[0].Id)
            {
                int stopIndex = chatMessages.Count - (chatMessages[chatMessages.Count - 1].Id - _sessionData.DataMessages[0].Id);
                for (int i = 0; i < stopIndex; i++)
                {
                    _sessionData.DataMessages.Add(chatMessages[i]);
                }
                _sessionData.SortList();
                return GetNewMessagesAndAllCountNotReadMessages(chatMessages);
            }

            for (int i = 0; i < chatMessages.Count; i++)
            {
                _sessionData.DataMessages.Add(chatMessages[i]);
            }
            int difference = _sessionData.DataMessages[0].Id - chatMessages[chatMessages.Count - 1].Id - 1;
            List<ChatMessage> newData = _efUnitOfWork.CertainMessages.GetMessagesAfterId(chatMessages[chatMessages.Count - 1].Id).Take(difference).ToList();
            foreach (var message in newData)
            {
                _sessionData.DataMessages.Add(GetReadyData(message));
            }
            _sessionData.SortList();
            return GetNewMessagesAndAllCountNotReadMessages(chatMessages);
        }

        public (List<ChatMessageDTO>, int) GetNewMessagesAndAllCountNotReadMessages(ChatMessageDTO chatMessage)
        {
            var list = new List<ChatMessageDTO>();

            int index = _sessionData.IndexOf(chatMessage);
            int startIndex = index + 1;
            for (int i = startIndex; i < _sessionData.DataMessages.Count; i++)
            {
                list.Add(_sessionData.DataMessages[i]);
            }
            return (list.Take(200).ToList(), list.Count - list.Take(200).ToList().Count);
        }

        public EFUnitOfWork GetUnitOfWork()
        {
            return _efUnitOfWork;
        }

        public async void SaveAsync()
        {
            await Task.Run(_efUnitOfWork.Save);
        }

        /// <summary>
        /// Метод для перевода данных в объект, который может быть передан клиенту
        /// </summary>
        /// <param name="chatMessage"> Объект модели базы данных </param>
        /// <returns> Объект, который можно передавать клиенту </returns>
        private ChatMessageDTO GetReadyData(ChatMessage chatMessage)
        {
            return new ChatMessageDTO
            {
                Id = chatMessage.IndexMessage,
                SenderName = chatMessage.User.Name,
                DateSend = chatMessage.TimeSendMessage.ToString(),
                SendMessage = chatMessage.Message
            };
        }
    }
}