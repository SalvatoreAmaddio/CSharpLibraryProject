using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Worksheet : IDestroyable
    {
        XL._Worksheet wrksheet;
        
        public Worksheet(XL._Worksheet wrksheet) 
        {
            this.wrksheet = wrksheet;
        }

        public void SetName(string name)
        {
            this.wrksheet.Name = name;
        }

        public void SetValue(int row, char col, object value)
        {
            wrksheet.Cells[row, col] = value;
        }

        public Range GetRange(string cell1, string cell2)
        {
            return new Range(wrksheet, cell1, cell2);
        }

        public void Destroy() => Marshal.ReleaseComObject(this.wrksheet);

    }
}
