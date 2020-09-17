using System;

namespace Server.DataTransferObject
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        public string Gender { get; set; }

        public string Name { get; set; }
    }
}