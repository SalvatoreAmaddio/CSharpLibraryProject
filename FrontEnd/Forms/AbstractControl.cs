using System.Windows.Controls;
using System.Windows;

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
}
