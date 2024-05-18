using FrontEnd.ExtensionMethods;
using FrontEnd.Model;
using System.Windows;

namespace MyApplication.View
{
    public partial class LoginForm : Window
    {

        User user = new User();
        public LoginForm()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //check if credentials were saved.
        }

        private void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            user.UserName = userName.Text;
            user.Password = pswd.Password;
            user.RememberMe = (bool)rememberme.IsChecked;
            bool result = user.Login();

            if (result) 
                this.GoToWindow(new MainWindow());
            else
            {
                AttemptRow.Height = new(30);
                if (user.Attempts == 0) Close(); //close application                 
                if (user.Attempts == 1)
                    attemptsLeft.Content = $"{user.Attempts} ATTEMPT LEFT!";
                else
                    attemptsLeft.Content = $"{user.Attempts} attempt(s) left.";
            }
        }

    }
}