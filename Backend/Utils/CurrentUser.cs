using Backend.Model;

namespace Backend.Utils
{
    public class CurrentUser
    {
        public static IUser? Is { private get; set; }
        
        public static string UserName 
        { 
            get 
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.UserName;
            }
            set 
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                Is.UserName = value;
            }
        }

        public static string Password
        {
            get
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.Password;
            }
            set
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                Is.Password = value;
            }
        }

        public static bool RememberMe
        {
            get
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.RememberMe;
            }
            set
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                Is.RememberMe = value;
            }
        }

        public static int Attempts
        {
            get
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.Attempts;
            }
        }

        public static string Target 
        { 
            get 
            {
                if (Is== null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.Target;
            }
        }

        public static bool Login(string pwd) 
        {
            if (Is == null)
                throw new ArgumentNullException(nameof(Is));
            return Is.Login(pwd);
        }

        public static void Logout()
        {
            if (Is == null)
                throw new ArgumentNullException(nameof(Is));
            Is.Logout();
        }
    }
}
