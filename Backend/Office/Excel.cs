using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using Backend.Exceptions;

namespace Backend.Office
{
    public class Excel : IDestroyable
    {
        Application? xlApp;
        Workbook? wrkbk;
        public Worksheet? Worksheet { get; private set; }
        public Range? Range { get; private set; }
       
        public void Create() 
        {
            xlApp = new XL.Application();
            if (xlApp == null) throw new MissingExcelException();
            wrkbk = new(xlApp);
            Worksheet = wrkbk.ActiveWorksheet;
        }

        public void Read(string path) 
        {
            xlApp = new XL.Application();
            if (xlApp == null) throw new MissingExcelException();
            wrkbk = new(xlApp, true, path);
        }

        /// <summary>
        /// This method calls <see cref="Workbook.Close"/>. Since it can throw a <see cref="WorkbookException"/>, wrap this method in a try-catch-finally block.
        /// For Example:
        /// <code>
        /// try 
        /// {
        ///     excel.Save("C:\\Users\\salva\\Desktop\\prova.xlsx");
        /// }
        /// catch (WorkbookException ex)
        /// {
        ///     return Task.FromException(ex); //return the exception.
        /// }
        /// finally 
        /// {
        ///     excel.Close(); //ensure the clean-up will always occur.
        /// }
        /// </code>
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath) => wrkbk?.Save(filePath);
        
        public void Close() 
        {
            xlApp?.Quit();
            Range?.Destroy();
            Worksheet?.Destroy();
            wrkbk?.Destroy();
            Destroy();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Destroy() 
        {
            if (xlApp == null) return;
            Marshal.ReleaseComObject(xlApp);
        }

    }
}