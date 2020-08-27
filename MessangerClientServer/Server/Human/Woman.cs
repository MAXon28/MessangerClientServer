using System;
using Server.Interface;

namespace Server.Human
{
    class Woman : IHuman
    {
        public string EntryInToTheChat(string name)
        {
            return name + " вошла в чат!";
        }

        public void SendMessage(string name)
        {
            Console.ForegroundColor = GetForeground();
            Console.Write(name);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" написала сообщение в чат!");
        }

        public string OutOfTheChat(string name)
        {
            Console.ForegroundColor = GetForeground();
            Console.Write(name);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" покинула чат!");
            return name + " покинула чат!";
        }

        public ConsoleColor GetForeground()
        {
            return ConsoleColor.DarkRed;
        }
    }
}