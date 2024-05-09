using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontEnd.Reports
{
    public class ReportViewer : Control
    {
        static ReportViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportViewer), new FrameworkPropertyMetadata(typeof(ReportViewer)));
        }

        #region SelectedPage
        public static readonly DependencyProperty SelectedPageProperty =
         DependencyProperty.Register(nameof(SelectedPage), typeof(ReportPage), typeof(ReportViewer), new PropertyMetadata());
        public ReportPage SelectedPage
        {
            get => (ReportPage)GetValue(SelectedPageProperty);
            set => SetValue(SelectedPageProperty, value);
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
         DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ReportViewer), new PropertyMetadata());
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        #endregion

        //PrintGrids_Click();
        //PrintDialog printDialog = new PrintDialog();
        //if (printDialog.ShowDialog() == true)
        //{
        //    // This assumes you have a Grid named 'myGrid' you want to print
        //    printDialog.PrintVisual(Page, "Printing Grid");
        //}

        private void PrintGrids_Click()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Create a document
                FixedDocument fixedDoc = new FixedDocument();
//                fixedDoc.DocumentPaginator.PageSize = new Size(Page.PageWidth, Page.PageWidth);

                // Assume you have a list of Grids
                List<IReportPage> myGrids = [];
 //               myGrids.Add(Page);
//                Border.Child = null;
                foreach (ReportPage page in myGrids)
                {
                    // Create page content
                    var pageContent = new PageContent();
                    var fixedPage = new FixedPage();
                    fixedPage.Width = page.PageWidth;
                    fixedPage.Height = page.PageHeight;

                    // Assume grids are prepared with right dimensions (e.g., A4 size)
                    page.Measure(new Size(fixedPage.Width, fixedPage.Height));
                    page.Arrange(new Rect(new Point(), fixedPage.DesiredSize));
                    page.UpdateLayout();

                    // Add the grid to the FixedPage
                    FixedPage.SetLeft(page, 0);
                    FixedPage.SetTop(page, 0);
                    fixedPage.Children.Add(page);

                    // Add the FixedPage to the PageContent
                    ((IAddChild)pageContent).AddChild(fixedPage);

                    // Add the PageContent to the FixedDocument
                    fixedDoc.Pages.Add(pageContent);
                }

                printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Multi-Grid Document");
            }
        }

        //private void PrintFlowDocument_Click()
        //{
        //    PrintDialog printDialog = new PrintDialog();
        //    if (printDialog.ShowDialog() == true)
        //    {
        //        // Create the FlowDocument
        //        FlowDocument flowDoc = new FlowDocument();
        //        flowDoc.PagePadding = new Thickness(0); // Adjust padding as needed
        //        flowDoc.ColumnGap = 0;
        //        flowDoc.ColumnWidth = printDialog.PrintableAreaWidth; // Set the column width to the page width
        //        Pages myContentList = [];
        //        myContentList.Add(Page);
        //        Border.Child = null;
        //        // Assume you have a list of content (e.g., Grids) to add
        //        foreach (UIElement element in myContentList)
        //        {
        //            // Wrap each element in a BlockUIContainer to include it in the FlowDocument
        //            BlockUIContainer container = new(element);

        //            // Measure and arrange the element (important if coming from another context)
        //            element.Measure(new Size(printDialog.PrintableAreaWidth, double.PositiveInfinity));
        //            element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));

        //            flowDoc.Blocks.Add(container);
        //        }

        //        // Create a DocumentPaginator wrapper for the FlowDocument
        //        IDocumentPaginatorSource idpSource = flowDoc;
        //        printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing Flow Document");
        //    }
        //}

    }
    }
