using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends <see cref="TabItem"/> to slightly change its style.
    /// </summary>
    public partial class Tab : TabItem
    {
        ResourceDictionary resourceDict = new()
        {
            Source = new Uri("pack://application:,,,/FrontEnd;component/Themes/TabStyle.xaml")
        };

        public Tab() 
        {
            Style = (Style)resourceDict["TabStyle"];
        }
    }
}
