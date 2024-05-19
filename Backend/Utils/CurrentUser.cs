using Backend.Database;
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

        public static string? InterrogateDatabase() 
        {
            if (Is == null) throw new ArgumentNullException(nameof(Is));
            List<QueryParameter> para = [new(nameof(UserName), Is.UserName)];
            return DatabaseManager.Do.Find("User")?.Retrieve(null,para).Cast<IUser>().FirstOrDefault()?.Password;
        }

        public static bool Login(string? pwd) 
        {
            if (Is == null)
                throw new ArgumentNullException(nameof(Is));
            return Is.Login(pwd);
        }

        public static void ResetAttempts()
        {
            if (Is == null)
                throw new ArgumentNullException(nameof(Is));
            Is.ResetAttempts();
        }
        
        /// <summary>
        /// Attempts to get and read the <see cref="Credential"/> object.
        /// If found, it sets the <see cref="UserName"/> and <see cref="Password"/> properties.
        /// </summary>
        /// <returns>true if the credential was found and read.</returns>
        public static bool ReadCredential() 
        {
            Credential? credential = CredentialManager.Get(Target);
            if (credential == null) return false;
            UserName = credential.Username;
            Password = credential.Password;
            return true;
        }

        public static void Logout()
        {
            if (Is == null)
                throw new ArgumentNullException(nameof(Is));
            Is.Logout();
        }
    }
}
