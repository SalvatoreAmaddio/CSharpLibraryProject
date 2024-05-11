using FrontEnd.Controller;
using FrontEnd.Events;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{

    public interface IControllable 
    {
        public IAbstractController Controller
        {
            get;
            set;
        }

        public bool IsListController { get; }
        public event ControllerChangedEventHandler? ControllerChanged;
    }

    public abstract class AbstractControl : Control, IControllable
    {
        public event ControllerChangedEventHandler? ControllerChanged;
        public bool IsListController { get => Controller is IListController; }

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register(nameof(Controller), typeof(IAbstractController), typeof(AbstractControl), new PropertyMetadata(OnControllerChanged));
        public IAbstractController Controller
        {
            get => (IAbstractController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }
        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AbstractControl control = (AbstractControl)d;
            control.OnControllerChanged(control, new(e.OldValue, e.NewValue));
        }

        static AbstractControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));

        public AbstractControl() => ControllerChanged += OnControllerChanged;


        /// <summary>
        /// Custom logic that triggers when the <see cref="Controller"/> property has changed. This method gets called by the <see cref="OnControllerChanged(DependencyPropertyChangedEventArgs)"/> event.
        /// </summary>
        protected virtual void OnControllerChanged(object? sender, ControllerChangedArgs e) { }

    }

    public abstract class AbstractContentControl : ContentControl, IControllable
    {
        public event ControllerChangedEventHandler? ControllerChanged;

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
            DependencyProperty.Register(nameof(Controller), typeof(IAbstractController), typeof(AbstractContentControl), new PropertyMetadata(OnControllerChanged));
        
        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            AbstractContentControl control = (AbstractContentControl)d;
            control.OnControllerChanged(control, new(e.OldValue, e.NewValue));
        } 

        static AbstractContentControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractContentControl), new FrameworkPropertyMetadata(typeof(AbstractContentControl)));
        
        public AbstractContentControl() => ControllerChanged += OnControllerChanged;

        /// <summary>
        /// Custom logic that triggers when the <see cref="Controller"/> property has changed. This method gets called by the <see cref="OnControllerChanged(DependencyPropertyChangedEventArgs)"/> event.
        /// </summary>
        protected virtual void OnControllerChanged(object? sender, ControllerChangedArgs e) { }
    }

}
