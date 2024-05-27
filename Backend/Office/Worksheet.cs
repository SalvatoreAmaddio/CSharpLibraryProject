using Backend.Model;
using Backend.Source;
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

        public void PrintData(IEnumerable<ISQLModel> records, int row = 1)
        {
            int column = 1;
            foreach (ISQLModel record in records) 
            {
                foreach(ITableField tableField in record.GetAllTableFields()) 
                {
                    var ft = tableField.FieldType;
                    var t = tableField.GetType().Name;
                    if (tableField is FKField fk) 
                    {
                        var x= fk.GetValue();
                        var f = fk.PK.GetValue();
                        var tl = fk.Name;
                        SetValue(x, row, column);
                    }
                    else 
                    {
                        SetValue(tableField?.GetValue(), row, column);
                    }
                    column++;
                }
                column = 1;
                row++;
            }
        }

        public void SetValue(object? value, int row = 1, int col = 1)
        {
            this.wrksheet.Cells[row, col] = (value == null) ? string.Empty : value;
        }

        public void SetValue(object? value, int row=1, string col="A")
        {
            this.wrksheet.Cells[row, col] = (value == null) ? string.Empty : value;
        }

        public Range GetRange(string cell1, string cell2)
        {
            return new Range(wrksheet, cell1, cell2);
        }

        public void Destroy() => Marshal.ReleaseComObject(this.wrksheet);

    }
}
