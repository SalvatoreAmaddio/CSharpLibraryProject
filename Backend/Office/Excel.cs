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
            if (xlApp == null)
                throw new MissingExcelException();

            wrkbk = new(xlApp);
            Worksheet = wrkbk.ActiveWorksheet;
        }

        public void Save(string filePath) => wrkbk?.Save(filePath);
        
        public void SetWorkingRange(string cell1, string cell2) 
        {
            Range = Worksheet?.GetRange(cell1,cell2);
        }

        public void Close() 
        {
            xlApp?.Quit();
            Range?.Destroy();
            Worksheet?.Destroy();
            wrkbk?.Destroy();
            Destroy();
        }

        public void Destroy() 
        {
            if (xlApp == null) return;
            Marshal.ReleaseComObject(xlApp);
        }

    }
}