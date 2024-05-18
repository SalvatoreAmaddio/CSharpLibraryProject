
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

        public User() { }

        public virtual bool Login()
        {
            bool userCheck = UserName.Equals("salvatore");
            bool passwordCheck = Password.Equals("soloio59");
            if (userCheck && passwordCheck) // you are in.
            {
                if (RememberMe)
                    SaveCredentials();

                //this.GoToWindow(new MainWindow());
                //go to main window.
                return true;
            }
            Attempts--;
            return false;
        }
        
        public virtual void SaveCredentials() 
        { 
            
        }
    }
}
