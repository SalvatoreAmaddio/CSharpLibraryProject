using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Workbook : IExcelComponent
    {
        XL.Workbook wrkbk;
        public int Count => wrkbk.Worksheets.Count;

        public Workbook(XL.Application xlApp) => wrkbk = xlApp.Workbooks.Add();

        public _Worksheet SelectSheet(int index) => (_Worksheet)wrkbk.Worksheets[index];
        public void AddNew(string name = "") 
        {
            wrkbk.Worksheets.Add(After: wrkbk.Sheets[Count]);
            if (!string.IsNullOrEmpty(name)) 
                ActiveWorksheet().Name = name;
        }

        public void Save(string filePath) => wrkbk.SaveAs(filePath);
        public _Worksheet ActiveWorksheet() => (_Worksheet)wrkbk.ActiveSheet;

        public void Close() => wrkbk?.Close(); 

        public void Destroy() => Marshal.ReleaseComObject(wrkbk);
    }
}
