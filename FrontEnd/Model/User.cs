
using Backend.Utils;

namespace FrontEnd.Model
{
    public class User : AbstractNotifier
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _rememberme = false;
        public string UserName { get => _username; set => UpdateProperty(ref value, ref _username); }
        public string Password { get => _password; set => UpdateProperty(ref value, ref _password); }
        public bool RememberMe { get => _rememberme; set => UpdateProperty(ref value, ref _rememberme); }
        public int Attempts { get; protected set; } = 3;
        public string Target { get; set; } = "LOGIN";
        public User() { }

        /// <summary>
        /// It attempts to login.
        /// </summary>
        /// <param name="pwd">The password to be checked against</param>
        /// <returns>true if the login attempt was successful</returns>
        /// <exception cref="Exception"></exception>
        public virtual bool Login(string pwd)
        {
            if (string.IsNullOrEmpty(Password)) throw new Exception("Password is empty");
            if (Password.Equals(pwd))
            {
                if (RememberMe)
                    SaveCredentials();
                return true;
            }
            Attempts--;
            return false;
        }
        
        public virtual void DeleteCredential() 
        {
            CredentialManager.Delete(Target);
        }

        public virtual void SaveCredentials() 
        {
            CredentialManager.Store(new(Target, UserName, Password));
        }
    }
}
