using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Range : IExcelComponent
    {
        Microsoft.Office.Interop.Excel.Range rng;

        public Range(_Worksheet wrksheet, string cell1, string cell2)
        {
            rng = wrksheet.get_Range(cell1, cell2);
            rng.Font.FontStyle = System.Drawing.FontStyle.Bold;
        }

        public void Formula(string formula) => rng.Formula = formula;
        public void Bold(bool value) => rng.Font.Bold = value;
        public void Italic(bool value) => rng.Font.Italic = value;
        public void Underline() => rng.Font.Underline = XlUnderlineStyle.xlUnderlineStyleSingle;
        public void SetColor(Color color) => rng.Font.Color = ColorTranslator.ToOle(color);
        public void SetBackground(Color color) => rng.Font.Background = ColorTranslator.ToOle(color);
        public void HorizontalAlignment(XlHAlign align) => rng.HorizontalAlignment = align;
        public void VerticalAlignment(XlHAlign align) => rng.VerticalAlignment = align;
        public void Destroy() => Marshal.ReleaseComObject(rng);
    }
}
