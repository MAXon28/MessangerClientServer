using System.IO;

namespace ChatClient.Interface
{
    interface IViewModel
    {
        void Notification(BinaryReader binaryReader);
    }
}