﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace MyApplication.View
{
    public partial class Guide : Window
    {
        public Guide() => InitializeComponent();

        private void OnLinkClicked(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo info = new(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            };
            Process.Start(info);
            e.Handled = true;
        }
    }
}