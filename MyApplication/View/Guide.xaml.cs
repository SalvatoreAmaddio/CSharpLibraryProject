using System.Diagnostics;
using System.Windows;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for Guide.xaml
    /// </summary>
    public partial class Guide : Window
    {
        public Guide()
        {
            InitializeComponent();
        }

        private void OnLinkClicked(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ProcessStartInfo info = new(e.Uri.AbsoluteUri);
            info.UseShellExecute = true;
            Process.Start(info);
            e.Handled = true;
        }
    }
}
