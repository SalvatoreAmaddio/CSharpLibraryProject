using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Ex
    {
        Application? xlApp;
        Workbook? wrkbk;
        _Worksheet? wrksheet;
        Range? rng;
        public Ex() 
        {
            

        }

        public void Create() 
        {
            xlApp = new Excel.Application();
            if (xlApp == null)
            {
                throw new Exception("Excel is not properly installed.");
            }
        }

        public void AddWorkBook() 
        {
            if (xlApp == null) throw new Exception($"{xlApp} was not created or Microsoft Excel is not installed in the local computer");
            wrkbk = xlApp.Workbooks.Add();
            wrksheet = (_Worksheet)wrkbk.ActiveSheet;
        }

        public void Save(string filePath) 
        {
            if (wrkbk == null) throw new Exception($"{wrkbk} was not created.");
            wrkbk.SaveAs(filePath);
        }

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
            if (wrkbk == null || xlApp == null || wrksheet == null) return;

            rng?.Destroy();
            Marshal.ReleaseComObject(wrksheet);
            Marshal.ReleaseComObject(wrkbk);
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
