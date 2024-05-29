using Backend.Utils;
using FrontEnd.Dialogs;
using FrontEnd.Model;
using FrontEnd.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyApplication
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = user.Text;
            string password = pswd.Password;
            string confirm_password = pswd2.Password;

            if (string.IsNullOrEmpty(username)) 
            {
                Failure.Throw("Please provide a username");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                Failure.Throw("Please provide a password");
                return;
            }

            if (!password.Equals(confirm_password)) 
            {
                Failure.Throw("Passwords do not match");
                return;
            }

            User new_user = new()
            {
                UserName = username,
                Password = password
            };

            CurrentUser.SaveNewUser(new_user);
            SuccessDialog.Display("Amazing! Please login with your new credentials.");
            Sys.UpdateFirstTimeLogin(false);
            Close();
        }
    }
}
