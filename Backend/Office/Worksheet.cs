using Backend.Database;
using Backend.Model;
using Backend.Source;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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

        public void PrintHeader(string[] headers, int row = 1)
        {
            int column = 1;
            foreach(string header in headers) 
            {
                SetValue(header, row, column);
                column++;
            }

            Range range = GetRange(1, row, headers.Length, row);
            range.HorizontalAlignment(XlAlign.Center);
            range.VerticalAlignment(XlAlign.Center);
            range.Bold(true);
            range.Destroy();
        }

        public void PrintData(IEnumerable<ISQLModel> records, int row = 2)
        {
            int column = 1;
            foreach (ISQLModel record in records) 
            {
                foreach(ITableField tableField in record.GetAllTableFields()) 
                {
                    if (tableField is FKField fk) 
                    {
                        ISQLModel? value = DatabaseManager.Find(fk.ClassName)?.Records.FirstOrDefault(s=>s.Equals(fk?.GetValue()));
                        SetValue(value, row, column);
                    }
                    else 
                        SetValue(tableField?.GetValue(), row, column);
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

        public Range GetRange(int col1 = 1, int row1 = 1, int col2 = 1, int row2 = 1)
        {
            return new Range(wrksheet, col1, row1, col2, row2);
        }

        public Range GetRange(string cell1, string cell2)
        {
            return new Range(wrksheet, cell1, cell2);
        }

        public void Destroy() => Marshal.ReleaseComObject(this.wrksheet);

    }
}
