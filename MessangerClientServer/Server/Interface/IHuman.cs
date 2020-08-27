using System;

namespace Server.Interface
{
    interface IHuman
    {
        string EntryInToTheChat(string name);

        void SendMessage(string name);

        string OutOfTheChat(string name);

        ConsoleColor GetForeground();
    }
}