using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Events;
using System.Collections;
using System.Diagnostics;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class instantiate a control that allow the view and printing of Reports.
    /// <para/>
    /// see also: <seealso cref="ReportPage"/>, <seealso cref="ListPage"/>
    /// </summary>
    public class ReportViewer : Control
    {
        static ReportViewer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportViewer), new FrameworkPropertyMetadata(typeof(ReportViewer)));
        Button? PART_SendButton;

        public event SendEmailEventHandler? SendEmail;
        public ReportViewer()
        {
            PrintCommand = new CMDAsync(PrintFixDocs);
            Binding binding = new("PDFPrinterManager.NewPortName")
            {
                Source = this
            };
            SetBinding(FileNameProperty, binding);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_SendButton = (Button?)GetTemplateChild(nameof(PART_SendButton));
            if (PART_SendButton!=null)
                PART_SendButton.Click += PART_SendButton_Click;
        }

        private void PART_SendButton_Click(object sender, RoutedEventArgs e) => SendEmail?.Invoke(this,e);


        /// <summary>
        /// A PDFPrinterManager that manages the PDF Printer's port.
        /// </summary>
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
        /// <summary>
        /// Gets and sets the name of the file to be printed.
        /// </summary>
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        #endregion

        #region SelectedPage
        public static readonly DependencyProperty SelectedPageProperty =
         DependencyProperty.Register(nameof(SelectedPage), typeof(ReportPage), typeof(ReportViewer), new PropertyMetadata());
        /// <summary>
        /// The currently selected page of the Report.
        /// </summary>
        public ReportPage SelectedPage
        {
            get => (ReportPage)GetValue(SelectedPageProperty);
            set => SetValue(SelectedPageProperty, value);
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
         DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<ReportPage>), typeof(ReportViewer), new PropertyMetadata());
        /// <summary>
        /// An <see cref="IEnumerable"/> containing one or more <see cref="ReportPage"/>(s).
        /// </summary>
        public IEnumerable<ReportPage> ItemsSource
        {
            get => (IEnumerable<ReportPage>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        #endregion

        #region PrintCommand
        public static readonly DependencyProperty PrintCommandProperty =
         DependencyProperty.Register(nameof(PrintCommand), typeof(ICommand), typeof(ReportViewer), new PropertyMetadata());
        /// <summary>
        /// Command that calls the <see cref="PrintFixDocs"/> methodd to print the Document.
        /// </summary>
        public ICommand PrintCommand
        {
            get => (ICommand)GetValue(PrintCommandProperty);
            set => SetValue(PrintCommandProperty, value);
        }
        #endregion

        #region SendCommand
        public static readonly DependencyProperty SendCommandProperty =
         DependencyProperty.Register(nameof(SendCommand), typeof(ICommand), typeof(ReportViewer), new PropertyMetadata());
        public ICommand SendCommand
        {
            get => (ICommand)GetValue(SendCommandProperty);
            set => SetValue(SendCommandProperty, value);
        }
        #endregion

        private Task<TaskRes> Task1(double PageWidth, double PageHeight) 
        {
            TaskRes res = new();
            PDFPrinterManager.SetPort();
            PrintQueue? pdfPrinter =
            new LocalPrintServer()
           .GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections })
           .FirstOrDefault(pq => pq.Name.Contains("PDF"));

            if (pdfPrinter == null)
            {
                BrokenIntegrityDialog.Throw("I could not find a PDF Printer in your computer");
                PDFPrinterManager.ResetPort();
                return Task.FromResult(res);
            }
            
            PrintDialog printDialog = new()
            {
                PrintQueue = pdfPrinter
            };
            res.pdfPrinter = pdfPrinter;
            res.printDialog = printDialog;   
            res.Done = true;
            FixedDocument doc = new();
            doc.DocumentPaginator.PageSize = new Size(PageWidth, PageHeight);
            res.doc = doc;
            return Task.FromResult(res);
        }

        private Task<IEnumerable<PageContent>> CopySource() 
        {
            List<PageContent> pages = [];
            foreach (IClonablePage clone in ItemsSource.Cast<IClonablePage>())
            {
                ReportPage page = clone.CloneMe();
                pages.Add(page.AsPageContent());
            }
            return Task.FromResult<IEnumerable<PageContent>>(pages);
        }

        /// <summary>
        /// It checks if the Printing process has terminated before changing the Printer's port.
        /// </summary>
        /// <param name="printQueue"></param>
        /// <returns>A Task</returns>
        private static Task PrintingCompleted(PrintQueue printQueue) 
        {
            while (printQueue.NumberOfJobs > 0)
                printQueue.Refresh();                
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts the printing process.
        /// </summary>
        private async Task PrintFixDocs()
        {
            if (string.IsNullOrEmpty(FileName)) 
            {
                BrokenIntegrityDialog.Throw("Please, specify a file name");
                return;
            }

            IsLoading = true;

            await Task.Delay(1000);
            ReportPage first_page = ItemsSource.First();
            double width = first_page.PageWidth;
            double height = first_page.PageHeight;
            
            var prep = Task1(width, height);
            var copied =  Application.Current.Dispatcher.InvokeAsync(CopySource);

            await Task.WhenAll(prep, copied.Task);
                        
            if (!prep.Result.Done) goto EndExec;

            await Application.Current.Dispatcher.InvokeAsync(() => 
            {
                foreach (var i in copied.Result.Result)
                {
                    prep.Result.doc.Pages.Add(i);
                }

                prep.Result.printDialog.PrintDocument(prep.Result.doc.DocumentPaginator, "Printing Doc");
            });

            await PrintingCompleted(prep.Result.pdfPrinter);

            PDFPrinterManager.ResetPort();

            if (OpenFile)
                await Task.Run(()=>Open(PDFPrinterManager.FilePath));
            
            EndExec:
            await Task.Delay(1000);
            IsLoading = false;
        }
        
        /// <summary>
        /// Open the file after it has been printed.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public static void Open(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open the PDF file. Error: {ex.Message}");
            }
        }
    }


    public class TaskRes() 
    {
        public bool Done { get; set; } = false;
        public PrintQueue pdfPrinter { get; set; }
        public FixedDocument doc { get; set; }
        public PrintDialog printDialog { get; set; }
    }
}