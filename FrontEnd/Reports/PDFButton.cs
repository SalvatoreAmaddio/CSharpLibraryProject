using FrontEnd.Utils;
using System.Windows.Controls;

namespace FrontEnd.Reports
{
    public class PDFButton : Button
    {
        public PDFButton() 
        {
            ToolTip = "Print";
            Content = new Image()
            {
                Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/pdf.png")
            };

        }
    }
}
