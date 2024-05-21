using Backend.Utils;
using System.Windows.Controls;
using System.Windows;
using FrontEnd.Utils;

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
                SysCredentailTargets.EmailApp = Username;
                Encrypter encrypter = new(credential.Password, SysCredentailTargets.EmailAppEncrypterKey, SysCredentailTargets.EmailAppEncrypterIV);
                PART_Password.Password = encrypter.Decrypt();
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
        PasswordBox? PART_OldPassword;
        PasswordBox? PART_NewPassword;
        Button? PART_Change;
        static ChangeUserPasswordDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ChangeUserPasswordDialog), new FrameworkPropertyMetadata(typeof(ChangeUserPasswordDialog)));
       
        public ChangeUserPasswordDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_OldPassword = (PasswordBox?)GetTemplateChild(nameof(PART_OldPassword));
            PART_NewPassword = (PasswordBox?)GetTemplateChild(nameof(PART_NewPassword));
            PART_Change = (Button?)GetTemplateChild(nameof(PART_Change));

            if (PART_Change!=null)
                PART_Change.Click += OnChangeClicked;
        }

        private void OnChangeClicked(object sender, RoutedEventArgs e)
        {
            if (PART_OldPassword == null) throw new Exception($"Failed to fetch {PART_OldPassword}");
            if (!CurrentUser.Password.Equals(PART_OldPassword.Password))
            {
                BrokenIntegrityDialog.Throw("The old Password does not match the current Password.","Wrong Input");
                PART_OldPassword.Password = string.Empty;
                return;
            }

            if (PART_NewPassword == null) throw new Exception($"Failed to fetch {PART_NewPassword}");

            CurrentUser.ChangePassword(PART_NewPassword.Password);

            MessageBox.Show("Password Updated! You will need to login again when you launch the Application.","Done!");
            Close();
        }
    }
}