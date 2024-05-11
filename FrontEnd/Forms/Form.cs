using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract class representing a Form Object.
    /// </summary>
    public abstract class AbstractForm() : AbstractContentControl
    {
        #region IsLoading
        /// <summary>
        /// Gets and Sets the <see cref="ProgressBar.IsIndeterminate"/> property.
        /// </summary>
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

        #region MenuRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Menu"/> property.
        /// </summary>
        public GridLength MenuRow
        {
            get => (GridLength)GetValue(MenuRowProperty);
            set => SetValue(MenuRowProperty, value);
        }

        public static readonly DependencyProperty MenuRowProperty =
            DependencyProperty.Register(nameof(MenuRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        #region HeaderRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Header"/> property.
        /// </summary>
        public GridLength HeaderRow
        {
            get => (GridLength)GetValue(HeaderRowProperty);
            set => SetValue(HeaderRowProperty, value);
        }

        public static readonly DependencyProperty HeaderRowProperty =
            DependencyProperty.Register(nameof(HeaderRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        #region RecordTrackerRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="FormComponents.RecordTracker"/> object.
        /// </summary>
        public GridLength RecordTrackerRow
        {
            get => (GridLength)GetValue(RecordTrackerRowProperty);
            set => SetValue(RecordTrackerRowProperty, value);
        }

        public static readonly DependencyProperty RecordTrackerRowProperty =
            DependencyProperty.Register(nameof(RecordTrackerRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(30), null));
        #endregion

        static AbstractForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractForm), new FrameworkPropertyMetadata(typeof(AbstractForm)));
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

        protected override void OnControllerChanged(object? sender, ControllerChangedArgs e)
        {
            Binding isLoadingBinding = new(nameof(IsLoading))
            {
                Source = e.NewValue,
            };
            SetBinding(IsLoadingProperty, isLoadingBinding);

            Binding controllerBinding = new(nameof(Controller))
            {
                Source = this,
            };
            SetBinding(DataContextProperty, controllerBinding);
        }
    }

    /// <summary>
    /// This class initiates a Form object meant to deal with a <see cref="Lista"/> object.
    /// <para/>
    /// A Form List object comes with a <see cref="FormComponents.RecordTracker"/>
    /// </summary>
    public class FormList : AbstractForm
    {
        static FormList() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormList), new FrameworkPropertyMetadata(typeof(FormList)));

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent is not Lista) throw new Exception();
            base.OnContentChanged(oldContent, newContent);
        }

        protected override void OnControllerChanged(object? sender, ControllerChangedArgs e)
        {
            base.OnControllerChanged(sender, e);
        }
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

        static Form() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Form), new FrameworkPropertyMetadata(typeof(Form)));
    }

    /// <summary>
    /// This class instantiate a FormRow object which is used by a <see cref="Lista"/>'s <see cref="DataTemplate"/>.
    /// <para/>
    /// The <see cref="FormComponents.RecordTracker"/> object is disabled by default and should stay so.
    /// </summary>
    public class FormRow : Form 
    {
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
            DependencyProperty.Register(nameof(IsDirty), typeof(bool), typeof(FormRow), new PropertyMetadata());
        #endregion

        static FormRow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormRow), new FrameworkPropertyMetadata(typeof(FormRow)));

        public FormRow() => RecordTrackerRow = new(0);
    }
    public class SubForm : AbstractForm
    {
        static SubForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SubForm), new FrameworkPropertyMetadata(typeof(SubForm)));

        #region ParentRecord
        public ISQLModel ParentRecord
        {
            get => (ISQLModel)GetValue(ParentRecordProperty);
            set => SetValue(ParentRecordProperty, value);
        }

        public static readonly DependencyProperty ParentRecordProperty =
            DependencyProperty.Register(nameof(ParentRecord), typeof(ISQLModel), typeof(SubForm), new PropertyMetadata());

        protected override void OnControllerChanged(object? sender, ControllerChangedArgs e)
        {
            
        }
        #endregion


    }

    public class FormPresenter : ContentPresenter, IControllable
    {
        public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register(nameof(Controller), typeof(IAbstractController), typeof(FormPresenter), new PropertyMetadata(OnControllerChanged));
        public IAbstractController Controller
        {
            get => (IAbstractController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public bool IsListController { get => Controller is IListController; }

        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FormPresenter control = (FormPresenter)d;
            control.OnControllerChanged(control, new(e.OldValue, e.NewValue));
        }


        public event ControllerChangedEventHandler? ControllerChanged;

        public FormPresenter() => ControllerChanged += OnControllerChanged;

        protected virtual void OnControllerChanged(object? sender, ControllerChangedArgs e) 
        {
            AbstractForm form = (AbstractForm)Content;     
        }


    }
}