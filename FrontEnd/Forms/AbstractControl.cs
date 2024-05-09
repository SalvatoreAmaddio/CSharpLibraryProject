using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract Class that defines a set of Common Property and methods to be shared among Controls
    /// </summary>
    public abstract class AbstractControl : Control
    {
        /// <summary>
        /// Tells if the <see cref="Controller"> is a List Controller class. 
        /// </summary>
        public bool IsListController { get => Controller is IListController; }

        /// <summary>
        /// Gets and Sets the <see cref="IAbstractController"/> associated with this control.
        /// </summary>
        public IAbstractController Controller
        {
            get => (IAbstractController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register(nameof(Controller), typeof(IAbstractController), typeof(AbstractControl), new PropertyMetadata(OnControllerChanged));

        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((AbstractControl)d).OnControllerChanged(e);

        static AbstractControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));

        /// <summary>
        /// Override this method to set up a custom logic that triggers when the <see cref="Controller"/> property has changed.
        /// </summary>
        /// <param name="e"></param>
        protected abstract void OnControllerChanged(DependencyPropertyChangedEventArgs e);
    }

}
