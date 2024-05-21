using Backend.Utils;
using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Dialogs
{
    public class EmailAppDialog : Window
    {
        Button? PART_Save;
        PasswordBox? PART_Password;
        static EmailAppDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(EmailAppDialog), new FrameworkPropertyMetadata(typeof(EmailAppDialog)));

        #region Username
        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register(nameof(Username), typeof(string), typeof(EmailAppDialog), new PropertyMetadata(string.Empty));
        #endregion

        private Credential? credential;
        public EmailAppDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Save = (Button?)GetTemplateChild(nameof(PART_Save));
            PART_Password = (PasswordBox?)GetTemplateChild(nameof(PART_Password));
            if (PART_Password == null) throw new Exception($"Failed to fetch {nameof(PART_Password)}");
            if (PART_Save != null)
                PART_Save.Click += OnSaveClicked;

            credential = CredentialManager.Get(SysCredentailTargets.EmailApp);
            if (credential != null)
            {
                Username = credential.Username;
                PART_Password.Password = credential.Password;
            }
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            if (PART_Password == null) throw new Exception($"Failed to fetch {nameof(PART_Password)}");
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(PART_Password.Password))
            {
                BrokenIntegrityDialog.Throw("Username and/or Password have not been provided.");
                return;
            }

            SysCredentailTargets.EmailApp = Username;
            Encrypter encrypter = new(PART_Password.Password);
            CredentialManager.Store(new(SysCredentailTargets.EmailApp, Username, encrypter.Encrypt()));
            encrypter.ReplaceStoredKeyIV(SysCredentailTargets.EmailAppEncrypterKey, SysCredentailTargets.EmailAppEncrypterIV);
            MessageBox.Show("Saved!");
            Close();
        }
    }

    public class ChangeUserPasswordDialog : Window 
    {
        static ChangeUserPasswordDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ChangeUserPasswordDialog), new FrameworkPropertyMetadata(typeof(ChangeUserPasswordDialog)));
       
        public ChangeUserPasswordDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //Helper.ChangeUserPassword("soloio59");
        }

    }
}
