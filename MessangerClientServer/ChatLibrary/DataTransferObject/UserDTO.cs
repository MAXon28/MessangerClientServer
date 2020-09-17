using System;

namespace ChatLibrary.DataTransferObject
{
    [Serializable]
    public class UserDTO
    {
        public string Name { get; set; }

        public string Gender { get; set; }

        public DateTime? PastOnline { get; set; }
    }
}