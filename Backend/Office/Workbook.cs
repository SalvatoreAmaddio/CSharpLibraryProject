using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Workbook : IDestroyable
    {
        XL.Workbook wrkbk;
        public Worksheet ActiveWorksheet { get; private set; }
        public readonly List<Worksheet> Sheets = [];
        public int Count => Sheets.Count;

        public Workbook(XL.Application xlApp) 
        {
            wrkbk = xlApp.Workbooks.Add();
            Sheets.Add(new Worksheet((_Worksheet)wrkbk.ActiveSheet));
            ActiveWorksheet = Sheets[0];
        }

        public void SelectSheet(int index)
        {
            ActiveWorksheet = Sheets[index];
        }

        public void AddNewSheet(string name = "") 
        {
            Sheets.Add(new Worksheet((_Worksheet)wrkbk.Worksheets.Add(After: wrkbk.Sheets[Count])));
            ActiveWorksheet = Sheets[Sheets.Count-1];

            if (!string.IsNullOrEmpty(name)) 
                ActiveWorksheet.SetName(name);
        }

        public void Save(string filePath) 
        {
            try 
            {
                wrkbk.SaveAs(filePath);
                wrkbk.Close();
            }
            catch (COMException) 
            {
                throw new Exception("The file is open");
            }
        }

        public void Close() => wrkbk?.Close();

        public void Destroy() 
        {
            foreach (Worksheet sheet in Sheets) 
                sheet.Destroy();

            Sheets.Clear();
            Marshal.ReleaseComObject(wrkbk);
        } 
    }
}
