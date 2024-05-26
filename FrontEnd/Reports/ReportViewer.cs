using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Properties;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

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
        Button? PART_ChooseDir;

        /// <summary>
        /// Sets the <see cref="EmailSender"/> that will be used by the <see cref="OnSendEmailClicked(object?, EventArgs)"/>
        /// </summary>
        public EmailSender? EmailSender { private get; set;}

        public ReportViewer()
        {
            PrintCommand = new CMDAsync(PrintFixDocs);

            SetBinding(FileNameProperty, new Binding("PDFPrinterManager.NewPortName")
            {
                Source = this
            });

            SetBinding(DirNameProperty, new Binding("PDFPrinterManager.DirectoryPath")
            {
                Source = this
            });

            if (string.IsNullOrEmpty(FrontEndSettings.Default.ReportDefaultDirectory))
                FrontEndSettings.Default.ReportDefaultDirectory = Sys.Desktop;

            DirName = FrontEndSettings.Default.ReportDefaultDirectory;

        }

        private async void OnSendEmailClicked(object? sender, EventArgs e)
        {
            if (EmailSender == null) return;
            if (!EmailSender.CredentialCheck()) 
            {
                Failure.Throw("I could not find any email settings.");
                return;
            }
            DialogResult result = ConfirmDialog.Ask("Do you want to send this document by email?");
            if (result == DialogResult.No) return;
            bool openFile = OpenFile;
            OpenFile = false;
            Task<bool> t = PrintFixDocs();
            
            Message = "Preparing document...";
            await Task.Delay(100);

            EmailSender.AddAttachment(PDFPrinterManager.FilePath);
            bool hasPrinted = await t;
            if (!hasPrinted) return;
            IsLoading = true;
            await Task.Delay(100);

            Message = "Sending...";
            try
            {
                await Task.Run(EmailSender.SendAsync);
            }
            catch (Exception ex)
            {
                Failure.Throw("The system failed to send the email. Possible reasons could be:\n- Wrong email settings,\nPoor internet connection.");
                Message = "Email Task Failed.";
            }
            finally 
            {
                IsLoading = false;
                OpenFile = openFile;
                await Task.Run(() => File.Delete(PDFPrinterManager.FilePath));
            }
            Message = "Almost Ready...";
            SuccessDialog.Display("Email Sent");
            Message = "";
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_SendButton = (Button?)GetTemplateChild(nameof(PART_SendButton));
            if (PART_SendButton!=null)
                PART_SendButton.Click += OnSendEmailClicked;

            PART_ChooseDir = (Button?)GetTemplateChild(nameof(PART_ChooseDir));
            if (PART_ChooseDir!=null)
                PART_ChooseDir.Click += OnChooseDirClicked;
        }

        private void OnChooseDirClicked(object sender, RoutedEventArgs e)
        {
            FolderDialog folderDialog = new("Select the folder where to save this file.");
            bool result = folderDialog.ShowDialog();
            DirName = (result) ? folderDialog.SelectedPath : Sys.Desktop;
        }


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

        #region Message
        public static readonly DependencyProperty MessageProperty =
         DependencyProperty.Register(nameof(Message), typeof(string), typeof(ReportViewer), new PropertyMetadata(string.Empty));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
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

        #region DirName
        public static readonly DependencyProperty DirNameProperty =
         DependencyProperty.Register(nameof(DirName), typeof(string), typeof(ReportViewer), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDirNamePropertyChanged));

        private static void OnDirNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string? str = (string?)e.NewValue;
            ReportViewer reportViewer = (ReportViewer)d;
            reportViewer.DirName = (string.IsNullOrEmpty(str))? Sys.Desktop : str;
            FrontEndSettings.Default.ReportDefaultDirectory = reportViewer.DirName;
        }

        /// <summary>
        /// Gets and sets the name of the file to be printed.
        /// </summary>
        public string DirName
        {
            get => (string)GetValue(DirNameProperty);
            set => SetValue(DirNameProperty, value);
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

        private IEnumerable<PageContent> CopySource(IEnumerable<ReportPage> clonedPages)
        {
            foreach (ReportPage page in clonedPages)
                yield return page.AsPageContent();
        }

        /// <summary>
        /// It checks if the Printing process has terminated before changing the Printer's port.
        /// </summary>
        /// <param name="pdfPrinter"></param>
        /// <returns>A Task</returns>
        private Task PrintingCompleted(PrintQueue pdfPrinter) 
        {
            while (pdfPrinter.NumberOfJobs > 0)
                pdfPrinter.Refresh();                
            return Task.CompletedTask;
        }

        private PrintQueue? GetPDFPrinter()
        {
           return new LocalPrintServer()
               .GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections })
               .FirstOrDefault(pq => pq.Name.Contains("PDF"));
        }

        /// <summary>
        /// Starts the printing process.
        /// </summary>
        private async Task<bool> PrintFixDocs()
        {
            if (string.IsNullOrEmpty(FileName)) 
            {
                Failure.Throw("Please, specify a file name");
                return false;
            }
            ReportPage first_page = ItemsSource.First();
            double width = first_page.PageWidth;
            double height = first_page.PageHeight;
            PrintQueue? pdfPrinter = GetPDFPrinter();
            if (pdfPrinter == null)
            {
                Failure.Throw("I could not find a PDF Printer in your computer");
                return false;
            }

            PrintDialog printDialog = new()
            {
                PrintQueue = pdfPrinter
            };

            IsLoading = true;
            await Task.Delay(100);

            IEnumerable<ReportPage> clonedPages = ItemsSource.Cast<IClonablePage>().Select(s=>s.CloneMe());

            await Task.Run(PDFPrinterManager.SetPort);

            IEnumerable<PageContent> copied = await Task.Run(() => CopySource(clonedPages));

            FixedDocument doc = new();
            doc.DocumentPaginator.PageSize = new Size(width, height);

            Message = "Printing...";
            await Task.Delay(100);
            await Application.Current.Dispatcher.InvokeAsync(async() =>
            {
               await RunUI(copied, doc,printDialog,pdfPrinter);
            });


            if (OpenFile) 
            {
                Message = "Opening...";
                await Task.Run(() => Open(PDFPrinterManager.FilePath));
            }

            Message = "";
            IsLoading = false;
            return true;
        }

        private async Task RunUI(IEnumerable<PageContent> copied, FixedDocument doc, PrintDialog printDialog, PrintQueue pdfPrinter) 
        {
            foreach (var i in copied)
            {
                doc.Pages.Add(i);
            }
            printDialog.PrintDocument(doc.DocumentPaginator, "Printing Doc");
            Message = "Saving...";
            await PrintingCompleted(pdfPrinter);
            Message = "Almost Ready...";
            await Task.Run(PDFPrinterManager.ResetPort);
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
}