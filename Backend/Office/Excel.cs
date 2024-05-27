using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Excel
    {
        Application? xlApp;
        Workbook? wrkbk;
        _Worksheet? wrksheet;
        Range? rng;
       
        public void Create() 
        {
            xlApp = new XL.Application();
            if (xlApp == null)
            {
                throw new Exception("Excel is not properly installed.");
            }

            wrkbk = new(xlApp);
            wrksheet = wrkbk.ActiveWorksheet();
        }

        public void GetSheet(int index) 
        {
            wrksheet = wrkbk?.SelectSheet(index);
        }

        public void Save(string filePath) => wrkbk?.Save(filePath);

        public void SetValue(int row, char col, object value) 
        {
            if (wrksheet == null) throw new Exception($"{wrksheet} was not created.");
            wrksheet.Cells[row, col] = value;
        }
        
        public Range GetRange(string cell1, string cell2) 
        {
            if (wrksheet == null) throw new Exception("Worksheet was not created");
            rng = new Range(wrksheet,cell1,cell2);
            return rng;
        }

        public void Close() 
        {
            wrkbk?.Close();
            xlApp?.Quit();
            if (xlApp == null || wrksheet == null) return;

            rng?.Destroy();
            Marshal.ReleaseComObject(wrksheet);
            wrkbk?.Destroy();
            Marshal.ReleaseComObject(xlApp);
        }
    }
}