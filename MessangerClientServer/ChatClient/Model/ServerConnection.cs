using System;
using System.IO;
using System.Xml.Serialization;

namespace ChatClient.Model
{
    [Serializable]
    public class ServerConnection
    {
        private static ServerConnection _serverConnection;
        private static XmlSerializer _xmlSerializer = new XmlSerializer(typeof(ServerConnection));

        public static ServerConnection NewInstance()
        {
            if (_serverConnection == null)
            {
                _serverConnection = Deserialize();
            }
            return _serverConnection;
        }

        public string Ip { get; set; }

        public int Port { get; set; }

        public void Serialize()
        {
            using (var fileXML = new FileStream("DataServer.xml", FileMode.Create))
            {
                _xmlSerializer.Serialize(fileXML, _serverConnection);
            }
        }

        private static ServerConnection Deserialize()
        {
            try
            {
                using (FileStream fileXML = new FileStream("DataServer.xml", FileMode.Open))
                {
                    try
                    {
                        return (ServerConnection)_xmlSerializer.Deserialize(fileXML);
                    }
                    catch (Exception)
                    {
                        return new ServerConnection();
                    }
                }
            }
            catch (Exception)
            {
                FileStream fileXML = new FileStream("DataServer.xml", FileMode.Create);
                fileXML.Close();
                return new ServerConnection();
            }

        }
    }
}