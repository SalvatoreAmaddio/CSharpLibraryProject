using Backend.Utils;
using FrontEnd.Dialogs;
using FrontEnd.Model;
using System.Windows;

namespace MyApplication
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void OnGetMeInClicked(object sender, RoutedEventArgs e)
        {
            string username = user.Text;
            string password = pswd.Password;
            string confirm_password = pswd2.Password;

            if (string.IsNullOrEmpty(username)) 
            {
                Failure.Allert("Please provide a username");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                Failure.Allert("Please provide a password");
                return;
            }

            if (!password.Equals(confirm_password)) 
            {
                Failure.Allert("Passwords do not match");
                return;
            }

            User new_user = new()
            {
                UserName = username,
                Password = password
            };

            Sys.SaveNewUser(new_user);
            SuccessDialog.Display("Amazing! Please login with your new credentials.");
            Sys.UpdateFirstTimeLogin(false);
            Close();
        }
    }
}
