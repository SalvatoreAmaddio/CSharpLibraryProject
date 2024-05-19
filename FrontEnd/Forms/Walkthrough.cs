using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public class Walkthrough : ContentControl
    {
        static Walkthrough()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Walkthrough), new FrameworkPropertyMetadata(typeof(Walkthrough)));
        }
    }
}
