using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms.Controls
{
    public class LoadingMask : Control
    {
        static LoadingMask()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingMask), new FrameworkPropertyMetadata(typeof(LoadingMask)));
        }

        #region Content
        public FrameworkElement Content
        {
            get => (FrameworkElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(LoadingMask), new PropertyMetadata());
        #endregion

    }
}
