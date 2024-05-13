using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
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

        protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IAbstractFormController) 
            {
                Binding isLoadingBinding = new(nameof(IsLoading))
                {
                    Source = e.NewValue,
                };
                SetBinding(IsLoadingProperty, isLoadingBinding);
            }
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
        static FormRow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormRow), new FrameworkPropertyMetadata(typeof(FormRow)));

        public FormRow() => RecordTrackerRow = new(0);
    }
    
    /// <summary>
    /// This class instantiate a SubForm which can contain another <see cref="AbstractForm"/> in a <see cref="Form"/>.
    /// </summary>
    public class SubForm : ContentControl
    {
        private AbstractForm? abstractForm;

        private event ParentRecordChangedEventHandler? ParentRecordChangedEvent;
        static SubForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SubForm), new FrameworkPropertyMetadata(typeof(SubForm)));

        public SubForm() => ParentRecordChangedEvent += OnParentRecordChanged;

        private void OnParentRecordChanged(object? sender, ParentRecordChangedArgs e) => NotifyAbstractForm(e.OldValue, e.NewValue);

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            abstractForm = (AbstractForm?)newContent;
            if (abstractForm == null) throw new Exception("A SubForm can only contain an AbstractForm object.");
            abstractForm.DataContextChanged += OnAbstractFormDataContextChanged;
        }
        private void OnAbstractFormDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => NotifyAbstractForm(null, ParentRecord);
        private ISubFormController? GetController() => (ISubFormController?)abstractForm?.DataContext;

        private void NotifyAbstractForm(AbstractModel? oldRecord, AbstractModel? parentRecord) 
        {
            GetController()?.SetParentRecord(parentRecord);
            if (oldRecord != null)
                oldRecord.OnDirtyChanged -= OnParentRecordDirtyChanged;
            if (parentRecord!=null)
                parentRecord.OnDirtyChanged += OnParentRecordDirtyChanged;
            IsEnabled = (parentRecord == null) ? false : !parentRecord.IsNewRecord();
        }

        private void OnParentRecordDirtyChanged(object? sender, OnDirtyChangedEventArgs e) => IsEnabled = !e.Model.IsNewRecord();

        #region ParentRecord
        /// <summary>
        /// Gets and Sets the <see cref="Form"/>'s <see cref="IAbstractFormController"/> CurrentRecord property which filter the records of the this SubForm.
        /// </summary>
        public AbstractModel ParentRecord
        {
            get => (AbstractModel)GetValue(ParentRecordProperty);
            set => SetValue(ParentRecordProperty, value);
        }

        public static readonly DependencyProperty ParentRecordProperty = DependencyProperty.Register(nameof(ParentRecord), typeof(AbstractModel), typeof(SubForm), new PropertyMetadata(OnParentRecordPropertyChanged));
        private static void OnParentRecordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SubForm)d).OnParentRecordChanged(d,new(e.OldValue, e.NewValue));
        #endregion

    }

}