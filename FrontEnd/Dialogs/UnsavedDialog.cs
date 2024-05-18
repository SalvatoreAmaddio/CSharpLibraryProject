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
    /// Custom Window Dialog to ask the user what action they would like to perform in case some data are missing.
    /// This dialog is usually call when the user attempt to leave a record without saving it.
    /// This class also calls the Win32 API to hide the close button.
    /// </summary>
    public class UnsavedDialog : Window
    {
        #region TextProperty
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(UnsavedDialog), new PropertyMetadata("You must save the record before performing any other action. Do you want to save the record?"));
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

        #region Win32 API for focus on Yes Button
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        private Button? yesButton;
        private Button? noButton;
        private DialogResult Result { get; set; } = 0;
        private UnsavedDialog() 
        {
            Width = 400;
            Height = 200;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Title = "Wait!";
            WindowStyle = WindowStyle.SingleBorderWindow;
            Loaded += OnLoaded;
            Owner = Helper.GetActiveWindow();
        }

        private Task<bool> SetFocus() 
        {
            return Task.FromResult((yesButton == null) ? false : yesButton.Focus());
        }

        /// <summary>
        /// Attempts to set the focus on the YesButton but it still does not work.
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await SetFocus();
            //use the Win32 API to remove the close button.
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            int currentStyle = GetWindowLong(hWnd, GWL_STYLE);
            SetWindowLong(hWnd, GWL_STYLE, currentStyle & ~WS_SYSMENU);

            //use the Win32 API to set the focus on the Yes Button.
            IntPtr buttonHandle = new WindowInteropHelper(this).Handle;
            SetForegroundWindow(buttonHandle);
            yesButton?.Focus();
            Keyboard.Focus(yesButton);
            yesButton.IsDefault = true;
        }

        static UnsavedDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnsavedDialog), new FrameworkPropertyMetadata(typeof(UnsavedDialog)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            yesButton = (Button?)GetTemplateChild("Yes");
            noButton = (Button?)GetTemplateChild("No");
            if (yesButton == null || noButton == null) throw new Exception("Failed to find the buttons");
            yesButton.Click += OnYesClicked;
            noButton.Click += OnNoClicked;
        }

        private void OnNoClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.No;
            DialogResult = false;
        }

        private void OnYesClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.Yes;
            DialogResult = true;
        }

        public static DialogResult Ask(string? text = null) 
        {
            UnsavedDialog missingData = new();
            if (!string.IsNullOrEmpty(text))
                missingData.Text = text;

            bool? result = missingData.ShowDialog();
            if (result == null) return Dialogs.DialogResult.None;
            return missingData.Result;
        }
    }
}