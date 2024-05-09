using FrontEnd.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
        static ReportViewer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportViewer), new FrameworkPropertyMetadata(typeof(ReportViewer)));
        
        public ReportViewer() => PrintCommand = new CMD(PrintFixDocs);

        #region FileName
        public static readonly DependencyProperty FileNameProperty =
         DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ReportViewer), new PropertyMetadata());
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        #endregion

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
         DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<ReportPage>), typeof(ReportViewer), new PropertyMetadata());
        public IEnumerable<ReportPage> ItemsSource
        {
            get => (IEnumerable<ReportPage>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        #endregion

        #region PrintCommand
        public static readonly DependencyProperty PrintCommandProperty =
         DependencyProperty.Register(nameof(PrintCommand), typeof(ICommand), typeof(ReportViewer), new PropertyMetadata());
        public ICommand PrintCommand
        {
            get => (ICommand)GetValue(PrintCommandProperty);
            set => SetValue(PrintCommandProperty, value);
        }
        #endregion

        //PrintGrids_Click();
        //PrintDialog printDialog = new PrintDialog();
        //if (printDialog.ShowDialog() == true)
        //{
        //    // This assumes you have a Grid named 'myGrid' you want to print
        //    printDialog.PrintVisual(Page, "Printing Grid");
        //}

        private void PrintFixDocs()
        {
            PrintDialog printDialog = new();

            if (printDialog.ShowDialog() == true)
            {
                // Create a document
                FixedDocument fixedDoc = new();
                ReportPage first_page = ItemsSource.First();
                fixedDoc.DocumentPaginator.PageSize = new Size(first_page.PageWidth, first_page.PageHeight);

                // Assume you have a list of Grids
                foreach (ReportPage page in ItemsSource)
                {
                    // Create page content
                    PageContent pageContent = new();
                    FixedPage fixedPage = new ()
                    {
                        Width = page.PageWidth,
                        Height = page.PageHeight
                    };

                    // Assume grids are prepared with right dimensions (e.g., A4 size)
                    page.Measure(new Size(fixedPage.Width, fixedPage.Height));
                    page.Arrange(new Rect(new Point(), fixedPage.DesiredSize));
                    page.UpdateLayout();

                    // Add the grid to the FixedPage
                    FixedPage.SetLeft(page, 0);
                    FixedPage.SetTop(page, 0);
                    fixedPage.Children.Add(page.Copy());

                    // Add the FixedPage to the PageContent
                    ((IAddChild)pageContent).AddChild(fixedPage);

                    // Add the PageContent to the FixedDocument
                    fixedDoc.Pages.Add(pageContent);
                }

                printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Printing");
            }
        }

    }
}
