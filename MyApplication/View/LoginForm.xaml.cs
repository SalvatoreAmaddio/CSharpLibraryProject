using System.Windows;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        int attempts = 3;
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
            var u = user.Text;
            var pass = pswd.Password;
            bool? rem = rememberme.IsChecked;

            //attempt login
            bool userCheck = u.Equals("salvatore");
            bool passwordCheck = pass.Equals("soloio59");

            if (userCheck && passwordCheck) // you are in.
            { 
                if (rem.HasValue && (bool)rem) 
                { 
                    //save credential
                }

                MessageBox.Show("Welcome");
                //go to main window.
            }
            else //attempt failed.
            {
                attempts--;
                if (attempts == 0) Close(); //close application                 
                if (attempts == 1)
                    attemptsLeft.Content = $"{attempts} ATTEMPT LEFT!";
                else
                    attemptsLeft.Content = $"{attempts} attempt(s) left.";
            }
        }

    }
}