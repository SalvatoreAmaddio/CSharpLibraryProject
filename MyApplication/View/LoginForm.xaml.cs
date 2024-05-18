using System.Windows;
using System.Windows.Input;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Psw.TextInput += Psw_TextInput;
            MessageBox.Show(Psw.Password);
        }

        private void Psw_TextInput(object sender, TextCompositionEventArgs e)
        {
            MessageBox.Show("Hey");
        }
    }
}
