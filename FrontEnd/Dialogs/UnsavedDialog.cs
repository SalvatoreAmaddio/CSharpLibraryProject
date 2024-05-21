using Backend.Utils;
using FrontEnd.Forms;
using FrontEnd.Utils;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace FrontEnd.Dialogs
{
    /// <summary>
    /// Enums that represents the return value of the <see cref="UnsavedDialog.Ask"/>
    /// </summary>
    public enum DialogResult
    { 
        None = 0,
        Ok = 1,
        Yes = 2,
        No = 3,
        Cancel = 4
    }

    /// <summary>
    /// This class also calls the Win32 API to hide the close button.
    /// </summary>
    public abstract class AbstractDialog : Window, IDisposable
    {
        protected bool _disposed = false;

        #region TextProperty
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(AbstractDialog), new PropertyMetadata("You must save the record before performing any other action. Do you want to save the record?"));
        #endregion

        #region Win32 API
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        #endregion

        #region Win32 API for focus on Button
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion
        
        /// <summary>
        /// The result returned by <see cref="Window.ShowDialog"/>. The default value is <see cref="DialogResult.None"/>
        /// </summary>
        protected DialogResult Result { get; set; } = 0;
        
        public AbstractDialog() 
        {
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.SingleBorderWindow;
            Owner = Helper.GetActiveWindow();
            Loaded += OnLoaded;
            Width = 400;
            SizeToContent = SizeToContent.Height;
        }

        public AbstractDialog(string? text, string? title) : this()
        {
            if (!string.IsNullOrEmpty(text))
                Text = text;

            if (!string.IsNullOrEmpty(title))
                Title = title;
        }

        /// <summary>
        /// Wrap up method to be called in a Static method of the child class.
        /// </summary>
        /// <param name="dialog">An instance of <see cref="AbstractDialog"/></param>
        /// <returns>A <see cref="DialogResult"/> enum</returns>
        protected static DialogResult _ask(AbstractDialog dialog) 
        {
            bool? result = dialog.ShowDialog();
            if (result == null) return Dialogs.DialogResult.None;
            return dialog.Result;
        }

        /// <summary>
        /// Removes the close button. Also, it attempts to set the focus on the YesButton but it still does not work.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //use the Win32 API to remove the close button.
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            int currentStyle = GetWindowLong(hWnd, GWL_STYLE);
            SetWindowLong(hWnd, GWL_STYLE, currentStyle & ~WS_SYSMENU);

            //use the Win32 API to set the focus on the Yes Button.
            IntPtr buttonHandle = new WindowInteropHelper(this).Handle;
            SetForegroundWindow(buttonHandle);
            ButtonToFocusOn()?.Focus();
            Keyboard.Focus(ButtonToFocusOn());
            try 
            {
                ButtonToFocusOn()!.IsDefault = true;
            }
            catch(NullReferenceException) 
            { 
            
            }
        }

        /// <summary>
        /// Override this method to set which Button should have the focus when the dialog opens.
        /// </summary>
        /// <returns>The button which should have the focus on.</returns>
        public abstract Button? ButtonToFocusOn();

        /// <summary>
        /// This method is called within the <see cref="OnApplyTemplate"/>. Override this method 
        /// to get and manage UI controls. <para/> 
        /// For Example:
        /// <code>
        /// okButton = (Button?)GetTemplateChild("Okay");
        /// if (okButton == null) throw new Exception("Failed to find the button");
        /// okButton.Click += OnOkClicked;
        /// </code>
        /// </summary>
        public abstract void OnLoadedTemplate();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnLoadedTemplate();
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Loaded -= OnLoaded;
            }

            _disposed = true;
        }

        ~AbstractDialog() => Dispose(false);
    }

    /// <summary>
    /// Custom Window Dialog to ask the user what action they would like to perform in case some data are missing.
    /// This dialog is usually called when the user attempt to leave a record without saving it.<para/>
    /// This dialog is used in: <see cref="Lista"/>, <see cref="Controller.IAbstractFormController.OnWindowClosing(object?, System.ComponentModel.CancelEventArgs)"/> 
    /// </summary>
    public class UnsavedDialog : AbstractDialog
    {
        protected Button? yesButton;
        protected Button? noButton;
        static UnsavedDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(UnsavedDialog), new FrameworkPropertyMetadata(typeof(UnsavedDialog)));

        protected UnsavedDialog(string? text = null, string? title = null) : base(text, title)
        { }

        public override Button? ButtonToFocusOn() => yesButton;

        protected virtual void OnNoClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.No;
            DialogResult = false;
        }

        protected virtual void OnYesClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.Yes;
            DialogResult = true;
        }

        public override void OnLoadedTemplate()
        {
            yesButton = (Button?)GetTemplateChild("Yes");
            noButton = (Button?)GetTemplateChild("No");
            if (yesButton == null || noButton == null) throw new Exception("Failed to find the buttons");
            yesButton.Click += OnYesClicked;
            noButton.Click += OnNoClicked;
        }
        public static DialogResult Ask(string? text = null, string? title = "Wait") => _ask(new UnsavedDialog(text, title));

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) 
            {
                if (yesButton!=null)
                    yesButton.Click -= OnYesClicked;
                if (noButton != null)
                    noButton.Click -= OnNoClicked;
            }
        }
    }

    /// <summary>
    /// Custom Window Dialog to tells the user they have missed some mandatory fields.
    /// This dialog is usually called when the user attempt to save a record without meeting Record's Integrity criteria. <para/>
    /// This dialog is used in: <see cref="Model.AbstractModel.AllowUpdate()"/> 
    /// </summary>
    public class BrokenIntegrityDialog : AbstractDialog
    {
        private Button? okButton;

        static BrokenIntegrityDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BrokenIntegrityDialog), new FrameworkPropertyMetadata(typeof(BrokenIntegrityDialog)));
        }

        private BrokenIntegrityDialog(string? text = null, string? title = null) : base(text, title)
        {
        }

        public override Button? ButtonToFocusOn() => okButton;

        public override void OnLoadedTemplate()
        {
            okButton = (Button?)GetTemplateChild("Okay");
            if (okButton == null) throw new Exception("Failed to find the button");
            okButton.Click += OnOkClicked;
        }

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.Ok;
            DialogResult = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (okButton != null)
                    okButton.Click -= OnOkClicked;
            }
        }

        public static DialogResult Throw(string? text = null, string? title = "Something is missing") => _ask(new BrokenIntegrityDialog(text, title));

    }

    /// <summary>
    /// This class extends <see cref="UnsavedDialog"/> and it is used to ask a user if they want to proceed with a given action or not.
    /// This dialog is usally used to ask a user if they want to delete a record or logout from the system.
    /// </summary>
    public class ConfirmDialog : UnsavedDialog
    {
        protected ConfirmDialog(string? text = null, string? title = null) : base(text, title)
        { 
        
        }

        static ConfirmDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConfirmDialog), new FrameworkPropertyMetadata(typeof(ConfirmDialog)));
        }

        public static new DialogResult Ask(string? text = null, string? title = "Confirm") => _ask(new ConfirmDialog(text, title));

    }

    public class EmailAppDialog : Window
    {
        Button? PART_Save;
        PasswordBox? PART_Password;
        static EmailAppDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EmailAppDialog), new FrameworkPropertyMetadata(typeof(EmailAppDialog)));
        }

        #region Username
        /// <summary>
        /// </summary>
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
}