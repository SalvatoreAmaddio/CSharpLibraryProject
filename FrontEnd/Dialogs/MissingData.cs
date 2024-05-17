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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FrontEnd.Dialogs
{
    public class MissingData : Window
    {
        private Button? yesButton;
        private Button? noButton;
        public MissingData() 
        {
            Width = 400;
            Height = 200;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Title = "Wait!";
            WindowStyle = WindowStyle.SingleBorderWindow;
            Loaded += MissingData_Loaded;
        }

        private Task<bool> SetFocus() 
        {
            return Task.FromResult((yesButton == null) ? false : yesButton.Focus());
        }
        private async void MissingData_Loaded(object sender, RoutedEventArgs e)
        {
            await SetFocus();
        }

        static MissingData()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MissingData), new FrameworkPropertyMetadata(typeof(MissingData)));
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
            DialogResult = false;
        }

        private void OnYesClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
