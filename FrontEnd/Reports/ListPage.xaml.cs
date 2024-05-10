using System.Windows.Controls;

namespace FrontEnd.Reports
{
    /// <summary>
    /// Interaction logic for ListPage.xaml
    /// </summary>
    public partial class ListPage : ListBox
    {
        public ListPage()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ScrollIntoView(SelectedItem);
    }
}
