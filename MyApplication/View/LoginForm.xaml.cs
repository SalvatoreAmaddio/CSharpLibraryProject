using Backend.Utils;
using FrontEnd.ExtensionMethods;
using FrontEnd.Model;
using System.Windows;

namespace MyApplication.View
{
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            CurrentUser.Is = new User();
            CurrentUser.KeyTarget = "EncrypterKey";
            CurrentUser.IVTarget = "EncrypterIV";
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.ReadCredential()) 
            {
                bool result = CurrentUser.Login(CurrentUser.FetchUserPassword());
                if (!result) 
                {
                    CredentialManager.Delete(CurrentUser.Target);
                    CurrentUser.ResetAttempts();
                    return;
                }
                this.GoToWindow(new MainWindow());
            }
        }

        private void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            CurrentUser.UserName = userName.Text;
            CurrentUser.Password = pswd.Password;
            CurrentUser.RememberMe = (bool)rememberme.IsChecked;
            string? p = CurrentUser.FetchUserPassword(true);
            if (CurrentUser.Login(p))
                this.GoToWindow(new MainWindow());
            else
            {
                InvalidCredentialRow.Height = new(30);
                AttemptRow.Height = new(30);
                if (CurrentUser.Attempts == 0) Close(); //close application                 
                attemptsLeft.Content = (CurrentUser.Attempts == 1) ? $"{CurrentUser.Attempts} ATTEMPT LEFT!" : $"{CurrentUser.Attempts} attempt(s) left.";
            }
        }    
    }
}