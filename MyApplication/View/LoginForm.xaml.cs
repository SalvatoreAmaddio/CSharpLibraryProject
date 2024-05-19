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
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.ReadCredential()) 
            {
                bool result = CurrentUser.Login(CurrentUser.InterrogateDatabase());
                if (!result) 
                {
                    CredentialManager.Delete(CurrentUser.Target);
                    CurrentUser.ResetAttempts();
                    return;
                }
                AttemptLogin(result);
            }
        }

        private void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            CurrentUser.UserName = userName.Text;
            CurrentUser.Password = pswd.Password;
            CurrentUser.RememberMe = (bool)rememberme.IsChecked;
            string? p = CurrentUser.InterrogateDatabase();
            AttemptLogin(CurrentUser.Login(p));
        }
    
        private void AttemptLogin(bool result) 
        {
            if (result)
                this.GoToWindow(new MainWindow());
            else
            {
                InvalidCredentialRow.Height = new(30);
                AttemptRow.Height = new(30);
                if (CurrentUser.Attempts == 0) Close(); //close application                 
                if (CurrentUser.Attempts == 1)
                    attemptsLeft.Content = $"{CurrentUser.Attempts} ATTEMPT LEFT!";
                else
                    attemptsLeft.Content = $"{CurrentUser.Attempts} attempt(s) left.";
            }
        }
    }
}