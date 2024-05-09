using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class ReportViewer : Page
    {
        public ReportViewer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // This assumes you have a Grid named 'myGrid' you want to print
//                printDialog.PrintVisual(Page, "Printing Grid");
            }
        }

        private void PrintGrids_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Create a document
                FixedDocument fixedDoc = new FixedDocument();
                fixedDoc.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                // Assume you have a list of Grids
                List<object> myGrids = [];
                foreach (Grid grid in myGrids)
                {
                    // Create page content
                    var pageContent = new PageContent();
                    var fixedPage = new FixedPage();
                    fixedPage.Width = fixedDoc.DocumentPaginator.PageSize.Width;
                    fixedPage.Height = fixedDoc.DocumentPaginator.PageSize.Height;

                    // Assume grids are prepared with right dimensions (e.g., A4 size)
                    grid.Measure(new Size(fixedPage.Width, fixedPage.Height));
                    grid.Arrange(new Rect(new Point(), fixedPage.DesiredSize));
                    grid.UpdateLayout();

                    // Add the grid to the FixedPage
                    FixedPage.SetLeft(grid, 0);
                    FixedPage.SetTop(grid, 0);
                    fixedPage.Children.Add(grid);

                    // Add the FixedPage to the PageContent
                    ((IAddChild)pageContent).AddChild(fixedPage);

                    // Add the PageContent to the FixedDocument
                    fixedDoc.Pages.Add(pageContent);
                }

                printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Multi-Grid Document");
            }
        }

        private void PrintFlowDocument_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Create the FlowDocument
                FlowDocument flowDoc = new FlowDocument();
                flowDoc.PagePadding = new Thickness(50); // Adjust padding as needed
                flowDoc.ColumnGap = 0;
                flowDoc.ColumnWidth = printDialog.PrintableAreaWidth; // Set the column width to the page width

                List<object> myContentList = [];
                // Assume you have a list of content (e.g., Grids) to add
                foreach (UIElement element in myContentList)
                {
                    // Wrap each element in a BlockUIContainer to include it in the FlowDocument
                    BlockUIContainer container = new BlockUIContainer(element);

                    // Measure and arrange the element (important if coming from another context)
                    element.Measure(new Size(printDialog.PrintableAreaWidth, double.PositiveInfinity));
                    element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));

                    flowDoc.Blocks.Add(container);
                }

                // Create a DocumentPaginator wrapper for the FlowDocument
                IDocumentPaginatorSource idpSource = flowDoc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing Flow Document");
            }
        }

    }
}
