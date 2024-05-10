using System.Windows.Controls;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class extends <see cref="ListBox"/> and it is meant to deal with <see cref="ReportPage"/> objects.
    /// It is used in <see cref="ReportViewer"/> to display Pages.
    /// </summary>
    public partial class ListPage : ListBox
    {
        public ListPage() => InitializeComponent();

        private void OnPageSelected(object sender, SelectionChangedEventArgs e) => ScrollIntoView(SelectedItem);
    }
}
