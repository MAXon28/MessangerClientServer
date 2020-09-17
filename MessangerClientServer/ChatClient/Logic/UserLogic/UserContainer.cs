using System.Security;

namespace ChatClient.Logic.UserLogic
{
    public class UserContainer
    {
        public static string Login { get; set; }

        public static SecureString Password { get; set; }

        public static string Gender { get; set; }
    }
}