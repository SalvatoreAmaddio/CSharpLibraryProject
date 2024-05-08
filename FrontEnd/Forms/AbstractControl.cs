using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public abstract class AbstractControl : Control
    {
        public bool IsListController { get => Controller is IListController; }
        public IAbstractController Controller
        {
            get => (IAbstractController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register(nameof(Controller), typeof(IAbstractController), typeof(AbstractControl), new PropertyMetadata(OnControllerChanged));

        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((AbstractControl)d).OnControllerChanged(e);

        protected abstract void OnControllerChanged(DependencyPropertyChangedEventArgs e);
        static AbstractControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));
        }
    }

}
