using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public abstract class AbstractControl : Control
    {
        static AbstractControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));

        public AbstractControl() => DataContextChanged += OnDataContextChanged;
        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public abstract class AbstractContentControl : ContentControl
    {
        static AbstractContentControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractContentControl), new FrameworkPropertyMetadata(typeof(AbstractContentControl)));

        public AbstractContentControl() => DataContextChanged += OnDataContextChanged;

        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

    }

}