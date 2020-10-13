using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ChatClient.Model;

namespace ChatClient.Logic.MessageLogic
{
    /// <summary>
    /// Класс-кэш, который хранит в себе данные о сообщениях, пока пользователь находится на других вкладках аккаунта
    /// </summary>
    public static class MessagesContainer
    {
        private static List<Message> _list;
        private static string _fileName;

        static MessagesContainer() { }

        public static void OpenContainer(string username)
        {
            _fileName = $"{username}Messages";
            var xmlSerializer = new XmlSerializer(typeof(List<Message>));
            using (var fileXML = new FileStream(_fileName, FileMode.Create))
            {
                try
                {
                    _list = (List<Message>)xmlSerializer.Deserialize(fileXML);
                }
                catch (Exception)
                {
                    _list = new List<Message>();
                }
            }
        }

        public static List<Message> GetMessages()
        {
            List<Message> messagesForView = new List<Message>(_list);
            var xmlSerializer = new XmlSerializer(typeof(List<Message>));
            using (var fileXML = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                try
                {
                    _list = (List<Message>)xmlSerializer.Deserialize(fileXML);
                }
                catch (Exception)
                {
                    _list = new List<Message>();
                }
            }
            return messagesForView;
        }

        public static void AddMessage(int id, string name, string date, string message, bool isItMe)
        {
            if (_list.Count == 100)
            {
                _list.RemoveAt(0);
            }
            _list.Add(new Message
            {
                Id = id,
                SenderName = name,
                DateSend = date,
                SendMessage = message,
                IsItMe = isItMe
            });
        }

        public static void SaveMessages()
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Message>));
            using (var fileXML = new FileStream(_fileName, FileMode.Create))
            {
                xmlSerializer.Serialize(fileXML, _list.Skip(_list.Count - 100).ToList());
            }
        }

        public static Message GetMessage()
        {
            return _list[_list.Count - 1];
        }
    }
}