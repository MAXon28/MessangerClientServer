using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ChatClient.Model;

namespace ChatClient.Logic.MessageLogic
{
    /// <summary>
    /// Класс, который занимается кэшированием данных о сообщениях в тот момент, когда пользователь находится непосредственно во вкладке "чат"
    /// </summary>
    public class Cache
    {
        private readonly string _nameCache;

        public Cache() {}

        public Cache(string name)
        {
            _nameCache = $"{name}MessagesCache";
            IndexLevel = 0;
            var fileMessagesXML = new FileStream(_nameCache, FileMode.Create);
            fileMessagesXML.Close();
        }

        public int IndexLevel { get; private set; }

        public void SerializeMessages(List<Message> list)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Message>));
            List<Message> listByAllLevels = DeserializeMessages();
            if (listByAllLevels.Count > 0)
            {
                foreach (var message in list)
                {
                    listByAllLevels.Add(message);
                }

                listByAllLevels = (from data in listByAllLevels
                    orderby data.Id
                    select data).ToList();
            }
            else
            {
                listByAllLevels = list;
            }
            using (var fileXML = new FileStream(_nameCache, FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fileXML, listByAllLevels);
                IndexLevel++;
            }
        }

        private List<Message> DeserializeMessages()
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Message>));
            using (var fileXML = new FileStream(_nameCache, FileMode.OpenOrCreate))
            {
                try
                {
                    return (List<Message>)xmlSerializer.Deserialize(fileXML);
                }
                catch (Exception)
                {
                    return new List<Message>();
                }
            }
        }

        public List<Message> DeserializeMessages(int code)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Message>));
            List<Message> list = new List<Message>();
            List<Message> result = new List<Message>();
            using (var fileXML = new FileStream(_nameCache, FileMode.Open))
            {
                list = (List<Message>)xmlSerializer.Deserialize(fileXML);
            }

            if (code == 0)
            {
                IndexLevel = 0;
                result = list.Skip(list.Count - 200).ToList();
                list.Clear();
            }
            else
            {
                IndexLevel--;
                result = list.Take(200).ToList();
                int count = 0;
                while (count < 200)
                {
                    list.RemoveAt(0);
                    if (list.Count == 0)
                    {
                        count = 200;
                    }
                    else
                    {
                        count++;
                    }
                }
            }

            using (var fileXML = new FileStream(_nameCache, FileMode.Create))
            {
                xmlSerializer.Serialize(fileXML, list);
            }

            return result;
        }
    }
}