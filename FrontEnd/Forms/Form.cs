using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract class representing a Form Object.
    /// </summary>
    public abstract class AbstractForm() : AbstractControl
    {
        /// <summary>
        /// Gets and Sets the <see cref="ProgressBar.IsIndeterminate"/> property.
        /// </summary>
        #region IsLoading
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(AbstractForm), new PropertyMetadata(false));
        #endregion

        #region Header
        /// <summary>
        /// Gets and Sets the Form Header.
        /// </summary>
        public UIElement Header
        {
            get => (UIElement)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(UIElement), typeof(AbstractForm), new PropertyMetadata(OnElementChanged));
        #endregion

        #region Menu
        /// <summary>
        /// Gets and sets a <see cref="System.Windows.Controls.Menu"/> object.
        /// </summary>
        public Menu Menu
        {
            get => (Menu)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(Menu), typeof(AbstractForm), new PropertyMetadata(OnElementChanged));
        #endregion

        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Menu"/> property.
        /// </summary>
        #region MenuRow
        public GridLength MenuRow
        {
            get => (GridLength)GetValue(MenuRowProperty);
            set => SetValue(MenuRowProperty, value);
        }

        public static readonly DependencyProperty MenuRowProperty =
            DependencyProperty.Register(nameof(MenuRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Header"/> property.
        /// </summary>
        #region HeaderRow
        public GridLength HeaderRow
        {
            get => (GridLength)GetValue(HeaderRowProperty);
            set => SetValue(HeaderRowProperty, value);
        }

        public static readonly DependencyProperty HeaderRowProperty =
            DependencyProperty.Register(nameof(HeaderRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="FormComponents.RecordTracker"/> object.
        /// </summary>
        #region RecordTrackerRow
        public GridLength RecordTrackerRow
        {
            get => (GridLength)GetValue(RecordTrackerRowProperty);
            set => SetValue(RecordTrackerRowProperty, value);
        }

        public static readonly DependencyProperty RecordTrackerRowProperty =
            DependencyProperty.Register(nameof(RecordTrackerRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(30), null));
        #endregion

        static AbstractForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractForm), new FrameworkPropertyMetadata(typeof(AbstractForm)));
        protected override void OnControllerChanged(DependencyPropertyChangedEventArgs e) => OnControllerSet((IAbstractController)e.NewValue);

        protected static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AbstractForm control = (AbstractForm)d;
            switch (true)
            {
                case true when e.Property.Equals(MenuProperty):
                    control.MenuRow = new(SetRow(e.NewValue, 20));
                    break;
                case true when e.Property.Equals(HeaderProperty):
                    control.HeaderRow = new(SetRow(e.NewValue, 40));
                    break;
            }
        }

        protected static int SetRow(object value, int height)
        {
            if (value == null) return 0;
            else return height;
        }

        /// <summary>
        /// Custom logic that triggers when the <see cref="Controller"/> property has changed. This method gets called by the <see cref="OnControllerChanged(DependencyPropertyChangedEventArgs)"/> event.
        /// </summary>
        /// <param name="controller"></param>
        public virtual void OnControllerSet(IAbstractController controller)
        {
            Binding binding = new()
            {
                Source = controller,
                Path = new PropertyPath(nameof(IsLoading)),
            };

            SetBinding(IsLoadingProperty, binding);
        }

    }

    /// <summary>
    /// This class initiates a Form object meant to deal with a <see cref="Lista"/> object.
    /// <para/>
    /// A Form List object comes with a <see cref="FormComponents.RecordTracker"/>
    /// </summary>
    public class FormList : AbstractForm
    {

        /// <summary>
        /// Gets and Sets the <see cref="Lista"/> object to display.
        /// </summary>
        #region List
        public Lista List
        {
            get => (Lista)GetValue(ListProperty);
            set => SetValue(ListProperty, value);
        }

        public static readonly DependencyProperty ListProperty =
        DependencyProperty.Register(nameof(List), typeof(Lista), typeof(FormList), new());
        #endregion

        static FormList() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormList), new FrameworkPropertyMetadata(typeof(FormList)));

    }

    /// <summary>
    /// This class initiates a Form object.
    /// <para/>
    /// A Form List object comes with a <see cref="FormComponents.RecordTracker"/> and a <see cref="FormComponents.RecordStatus"/> object.
    /// </summary>
    public class Form : AbstractForm
    {

        #region RecordStatusRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the width of a <see cref="FormComponents.RecordStatus"/> object.
        /// </summary>
        public GridLength RecordStatusColumn
        {
            get => (GridLength)GetValue(RecordStatusColumnProperty);
            set => SetValue(RecordStatusColumnProperty, value);
        }

        public static readonly DependencyProperty RecordStatusColumnProperty =
            DependencyProperty.Register(nameof(RecordStatusColumn), typeof(GridLength), typeof(Form), new PropertyMetadata(new GridLength(23), null));
        #endregion

        #region Content
        /// <summary>
        /// Gets and Sets the main Content that the Form displays.
        /// </summary>
        public FrameworkElement Content
        {
            get => (FrameworkElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(Form), new PropertyMetadata());
        #endregion

        #region IsDirty
        /// <summary>
        /// Gets and Sets a Flag that indicates if the current Record is being changed.
        /// </summary>
        public bool IsDirty
        {
            get => (bool)GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }

        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.Register(nameof(IsDirty), typeof(bool), typeof(Form), new PropertyMetadata());
        #endregion

        static Form() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Form), new FrameworkPropertyMetadata(typeof(Form)));

        public override void OnControllerSet(IAbstractController controller)
        {
            base.OnControllerSet(controller);
            Binding binding = new()
            {
                Source = controller,
                Path = new PropertyPath("CurrentModel.IsDirty"),
            };

            SetBinding(IsDirtyProperty, binding);
        }

    }

    /// <summary>
    /// This class instantiate a FormRow object which is used by a <see cref="Lista"/>'s <see cref="DataTemplate"/>.
    /// <para/>
    /// The <see cref="FormComponents.RecordTracker"/> object is disabled by default and should stay so.
    /// </summary>
    public class FormRow : Form 
    {
        static FormRow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormRow), new FrameworkPropertyMetadata(typeof(FormRow)));

        public FormRow() => RecordTrackerRow = new(0);
    }
}