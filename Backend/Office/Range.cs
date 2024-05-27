using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public enum XlAlign
    {
        Center = -4108,
        CenterAcrossSelection = 7,
        Distributed = -4117,
        Fill = 5,
        General = 1,
        Justify = -4130,
        Left = -4131,
        Right = -4152
    }

    public class Range : IDestroyable
    {
        XL.Range rng;

        public Range(_Worksheet wrksheet, string cell1, string cell2)
        {
            rng = wrksheet.get_Range(cell1, cell2);
        }

        public Range(_Worksheet wrksheet, int col1 = 1, int row1=1, int col2 = 1, int row2=1)
        {
            string cell1 = ConvertIndexToColumnLabel(col1) + row1.ToString();
            string cell2 = ConvertIndexToColumnLabel(col2) + row2.ToString();
            rng = wrksheet.get_Range(cell1, cell2);
        }

        public static string ConvertIndexToColumnLabel(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

        public void Formula(string formula) => rng.Formula = formula;
        public void Bold(bool value) => rng.Font.Bold = value;
        public void Italic(bool value) => rng.Font.Italic = value;
        public void Underline() => rng.Font.Underline = XlUnderlineStyle.xlUnderlineStyleSingle;
        public void SetColor(Color color) => rng.Font.Color = ColorTranslator.ToOle(color);
        public void SetBackground(Color color) => rng.Font.Background = ColorTranslator.ToOle(color);
        public void HorizontalAlignment(XlAlign align) => rng.HorizontalAlignment = align;
        public void VerticalAlignment(XlAlign align) => rng.VerticalAlignment = align;
        public void Destroy() => Marshal.ReleaseComObject(rng);
    }
}
