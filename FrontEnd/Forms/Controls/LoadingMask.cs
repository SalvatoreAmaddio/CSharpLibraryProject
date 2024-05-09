using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms.Controls
{
    /// <summary>
    /// This class instantiate the content for Window object to show a loading process
    /// </summary>
    public class LoadingMask : Control
    {
        static LoadingMask() => DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingMask), new FrameworkPropertyMetadata(typeof(LoadingMask)));

        #region Content
        /// <summary>
        /// The Content to be displayed
        /// </summary>
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
