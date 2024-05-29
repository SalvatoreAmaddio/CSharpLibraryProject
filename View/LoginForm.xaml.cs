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
            InitializeComponent();
            CurrentUser.Is = new User();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Sys.FirstTimeLogin)
            {
                new WelcomeWindow().ShowDialog();
                return;
            }

            if (CurrentUser.ReadCredential()) 
            {
                string? decryptedPassword = CurrentUser.FetchUserPassword(true);
                bool hasLoggedIn = CurrentUser.Login(decryptedPassword);
                if (!hasLoggedIn) 
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

            if (rememberme.IsChecked.HasValue && rememberme.IsChecked.Value) 
                CurrentUser.RememberMe = (bool)rememberme.IsChecked;
            
            string? decryptedPassword = CurrentUser.FetchUserPassword(true);
            bool hasLoggedIn = CurrentUser.Login(decryptedPassword);
            if (hasLoggedIn)
                this.GoToWindow(new MainWindow());
            else
            {
                ShowWarnings();
                if (CurrentUser.Attempts == 0) Close(); //close application                 
                attemptsLeft.Content = (CurrentUser.Attempts == 1) ? $"{CurrentUser.Attempts} ATTEMPT LEFT!" : $"{CurrentUser.Attempts} attempt(s) left.";
            }
        }    
    
        private void ShowWarnings() 
        {
            InvalidCredentialRow.Height = new(30);
            AttemptRow.Height = new(30);
        }
    }
}