using System;
using System.Collections.Generic;
using System.Linq;
using Server.EF;
using Server.Interface;
using Server.Model;

namespace Server.Repository
{
    class ChatMessageRepository : IRepository<ChatMessage>, ICertainList<ChatMessage>
    {
        private ChatDbContext _chatDbContext;

        public ChatMessageRepository(ChatDbContext db)
        {
            _chatDbContext = db;
        }

        public IEnumerable<ChatMessage> GetAll()
        {
            return _chatDbContext.ChatMessages;
        }

        public IEnumerable<ChatMessage> GetMessagesBeforeId(int lastId)
        {
            return from element in _chatDbContext.ChatMessages
                where element.IndexMessage < lastId
                select element;
        }

        public IEnumerable<ChatMessage> GetMessagesAfterId(int lastId)
        {
            return from element in _chatDbContext.ChatMessages
                where element.IndexMessage > lastId
                select element;
        }

        public void Create(ChatMessage message)
        {
            _chatDbContext.ChatMessages.Add(message);
        }
    }
}