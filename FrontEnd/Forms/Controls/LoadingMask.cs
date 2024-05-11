using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms.Controls
{
    /// <summary>
    /// This class instantiate the content for Window object to show a loading process
    /// </summary>
    public class LoadingMask : ContentControl
    {
        static LoadingMask() => DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingMask), new FrameworkPropertyMetadata(typeof(LoadingMask)));
    }
}
