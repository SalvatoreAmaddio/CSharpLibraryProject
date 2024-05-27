using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Excel : IDestroyable
    {
        Application? xlApp;
        Workbook? wrkbk;
        public Worksheet? wrksheet;
        public Range? rng;
       
        public void Create() 
        {
            xlApp = new XL.Application();
            if (xlApp == null)
                throw new Exception("Excel is not properly installed.");

            wrkbk = new(xlApp);
            wrksheet = wrkbk.ActiveWorksheet;
        }

        public void GetSheet(int index) => wrksheet = wrkbk?.SelectSheet(index);
        public void Save(string filePath) => wrkbk?.Save(filePath);
        
        public Range? GetRange(string cell1, string cell2) 
        {
            rng = wrksheet?.GetRange(cell1,cell2);
            return rng;
        }

        public void Close() 
        {
            wrkbk?.Close();
            xlApp?.Quit();

            rng?.Destroy();
            wrksheet?.Destroy();
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