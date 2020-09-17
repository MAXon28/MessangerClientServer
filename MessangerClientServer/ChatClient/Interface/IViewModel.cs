using System.IO;

namespace ChatClient.Interface
{
    interface IViewModel
    {
        string Condition { get; set; }

        string Name { get; set; }

        void Notification(BinaryReader binaryReader);
    }
}