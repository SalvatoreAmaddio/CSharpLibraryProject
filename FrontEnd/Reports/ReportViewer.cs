using FrontEnd.Controller;
using System.Collections;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace FrontEnd.Reports
{
    public class ReportViewer : Control
    {
        static ReportViewer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportViewer), new FrameworkPropertyMetadata(typeof(ReportViewer)));

        public ReportViewer() 
        {
            PrintCommand = new CMD(PrintFixDocs);
            Binding binding = new("PDFPrinterManager.FileName")
            {
                Source = this
            };
            SetBinding(FileNameProperty, binding);
        }
        public MicrosoftPDFPrinterManager PDFPrinterManager { get; } = new();

        #region OpenFile
        /// <summary>
        /// Sets this property to true to open the file after the printing process has completed.
        /// </summary>
        public bool OpenFile
        {
            get => (bool)GetValue(OpenFileProperty);
            set => SetValue(OpenFileProperty, value);
        }

        public static readonly DependencyProperty OpenFileProperty =
            DependencyProperty.Register(nameof(OpenFile), typeof(bool), typeof(ReportViewer), new PropertyMetadata(false));
        #endregion

        #region IsLoading
        /// <summary>
        /// Gets and Sets the <see cref="ProgressBar.IsIndeterminate"/> property.
        /// </summary>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(ReportViewer), new PropertyMetadata(false));
        #endregion

        #region FileName
        public static readonly DependencyProperty FileNameProperty =
         DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ReportViewer), new FrameworkPropertyMetadata(string.Empty,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
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

        private Task<IEnumerable<FixedPage>> PrintAsync(PrintQueue pdfPrinter) 
        {
            PrintDialog printDialog = new()
            {
                PrintQueue = pdfPrinter
            };

            ReportPage first_page = ItemsSource.First();
            FixedDocument doc = new();
            doc.DocumentPaginator.PageSize = new Size(first_page.PageWidth, first_page.PageHeight);
           
            foreach (ReportPage page in ItemsSource)
            {
                FixedPage fixedPage = new()
                {
                    Width = page.PageWidth,
                    Height = page.PageHeight
                };

                page.Measure(new Size(fixedPage.Width, fixedPage.Height));
                page.Arrange(new Rect(new Point(), fixedPage.DesiredSize));
                page.UpdateLayout();

                FixedPage.SetLeft(page, 0);
                FixedPage.SetTop(page, 0);

                fixedPage.Children.Add(page.Copy());

                PageContent pageContent = new();
                ((IAddChild)pageContent).AddChild(fixedPage);

                doc.Pages.Add(pageContent);
            }

            printDialog.PrintDocument(doc.DocumentPaginator, "Printing");

            return Task.FromResult(doc.Pages.Select(s => s.Child));
        }
        private static Task PrintingCompleted(PrintQueue printQueue) 
        {
            while (printQueue.NumberOfJobs > 0)
                printQueue.Refresh();                
            return Task.CompletedTask;
        }
        private async void PrintFixDocs()
        {
            if (string.IsNullOrEmpty(FileName)) 
            {
                MessageBox.Show("Please, specify a file name","Something is missing");
                return;
            }

            IsLoading = true;
            await Task.Delay(1000);
            LocalPrintServer printServer = new();
            PrintQueueCollection printQueues = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            PrintQueue? pdfPrinter = printQueues.FirstOrDefault(pq => pq.Name.Contains("PDF"));

            if (pdfPrinter == null) 
            {
                MessageBox.Show("I could not find a PDF Printer in your computer", "Something is missing");
                return;
            }

            await Task.Run(PDFPrinterManager.SetPort);
            await Dispatcher.BeginInvoke(async () => 
            {
                ItemsSource = ConvertToReportPages(await PrintAsync(pdfPrinter));
            });

            await PrintingCompleted(pdfPrinter);
            await Task.Run(PDFPrinterManager.ResetPort);
            if (OpenFile)
                await Task.Run(PDFPrinterManager.OpenFile);
            
            await Task.Delay(1000);
            IsLoading = false;
        }

        /// <summary>
        /// It extracts and disconnects the <see cref="ReportPage"/> from its <see cref="FixedPage"/>.
        /// </summary>
        /// <param name="fixedPages"></param>
        /// <returns>A list of orphan ReportPages</returns>
        private static List<ReportPage> ConvertToReportPages(IEnumerable<FixedPage> fixedPages)
        {
            List<ReportPage> pages = [];
            foreach(FixedPage fixedPage in fixedPages) 
            {
                ReportPage reportPage = (ReportPage)fixedPage.Children[0];
                fixedPage.Children.Clear(); //disconnect from FixedPage
                pages.Add(reportPage);
            }
            return pages;
        }

    }
}
