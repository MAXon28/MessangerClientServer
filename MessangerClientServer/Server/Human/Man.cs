using System;
using Server.Interface;

namespace Server.Human
{
    class Man : IHuman
    {
        public string EntryInToTheChat(string name)
        {
            return name + " вошёл в чат!";
        }

        public void SendMessage(string name)
        {
            Console.ForegroundColor = GetForeground();
            Console.Write(name);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" написал сообщение в чат!");
        }

        public string OutOfTheChat(string name)
        {
            Console.ForegroundColor = GetForeground();
            Console.Write(name);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" покинул чат!");
            return name + " покинул чат!";
        }

        public ConsoleColor GetForeground()
        {
            return ConsoleColor.Blue;
        }
    }
}