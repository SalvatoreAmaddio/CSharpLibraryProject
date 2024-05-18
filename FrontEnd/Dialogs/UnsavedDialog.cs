﻿using FrontEnd.Forms;
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
    public abstract class AbstractDialog : Window 
    {
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
            ButtonToFocusOn().IsDefault = true;
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
            this.OnLoadedTemplate();
        }
    }

    /// <summary>
    /// Custom Window Dialog to ask the user what action they would like to perform in case some data are missing.
    /// This dialog is usually called when the user attempt to leave a record without saving it.<para/>
    /// This dialog is used in: <see cref="Lista"/>, <see cref="Controller.IAbstractFormController.OnWindowClosing(object?, System.ComponentModel.CancelEventArgs)"/> 
    /// </summary>
    public class UnsavedDialog : AbstractDialog
    {
        private Button? yesButton;
        private Button? noButton;
        static UnsavedDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnsavedDialog), new FrameworkPropertyMetadata(typeof(UnsavedDialog)));
        }

        private UnsavedDialog(string? text = null, string? title = null) : base(text, title)
        {
        }

        public override Button? ButtonToFocusOn() => yesButton;

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

        public override void OnLoadedTemplate()
        {
            yesButton = (Button?)GetTemplateChild("Yes");
            noButton = (Button?)GetTemplateChild("No");
            if (yesButton == null || noButton == null) throw new Exception("Failed to find the buttons");
            yesButton.Click += OnYesClicked;
            noButton.Click += OnNoClicked;
        }
        public static DialogResult Ask(string? text = null, string? title = "Wait") => _ask(new UnsavedDialog(text, title));
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

        public static DialogResult Throw(string? text = null, string? title = "Something is missing") => _ask(new BrokenIntegrityDialog(text, title));

    }
}