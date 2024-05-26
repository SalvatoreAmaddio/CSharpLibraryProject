using Backend.Utils;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;

namespace FrontEnd.Reports
{
    public class PDFPrinter 
    {
        private PrintQueue Queue { get; set; }

        /// <summary>
        /// A PDFPrinterManager that manages the PDF Printer's port.
        /// </summary>
        public MicrosoftPDFPrinterPortManager PrinterPortManager { get; } = new();

        public PDFPrinter() 
        {
            Queue = GetPDFPrinter();        
        }

        public static bool IsInstalled() 
        {
            PrintQueue? pdfPrinter = new LocalPrintServer()
                .GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections })
                .FirstOrDefault(pq => pq.Name.Contains("PDF"));
            return pdfPrinter != null;
        }

        public PDFPrinter(string fileName, string dirPath) : this()
        {
            PrinterPortManager.NewPortName = fileName;
            PrinterPortManager.DirectoryPath = dirPath;
        }
        
        public void Print(DocumentPaginator documentPaginator) 
        {
            PrintTicket printTicket = Queue.DefaultPrintTicket;
            XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(Queue);
            xpsDocumentWriter.Write(documentPaginator, printTicket);
            while (Queue.NumberOfJobs > 0)
                Queue.Refresh();
        }

        public Task PrintAsync(DocumentPaginator documentPaginator)
        {
            PrintTicket printTicket = Queue.DefaultPrintTicket;
            XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(Queue);
            xpsDocumentWriter.WriteAsync(documentPaginator, printTicket);
            while (Queue.NumberOfJobs > 0)
                Queue.Refresh();

            return Task.CompletedTask;
        }

        private static PrintQueue GetPDFPrinter() =>
            new LocalPrintServer()
                .GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections })
                .First(pq => pq.Name.Contains("PDF"));

    }
}
