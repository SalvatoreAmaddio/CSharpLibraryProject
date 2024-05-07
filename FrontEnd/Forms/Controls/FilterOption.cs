using FrontEnd.Controller;
using FrontEnd.FilterSource;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace FrontEnd.Forms
{
    public class FilterOption : AbstractControl
    {
        private Popup? Popup;
        private Button? DropDownButton;
        private readonly Image Filter = new()
        {
            Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/filter.png")
        };
        private readonly Image ClearFilter = new()
        {
            Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/clear_filter.png")
        };

        static FilterOption()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterOption), new FrameworkPropertyMetadata(typeof(FilterOption)));
        }

        private void ResetDropDownButtonAppereance()
        {
            if (DropDownButton == null) throw new Exception("DropDownButton is null");
            DropDownButton.Content = Filter;
            ToolTip = "Filter";
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Popup = (Popup)GetTemplateChild("PART_popup");
            DropDownButton = (Button)GetTemplateChild("PART_dropdown_button");
            DropDownButton.Click += OnDropdownButtonClicked;
            ResetDropDownButtonAppereance();

            if (GetTemplateChild("PART_clear_button") is Button clearButton)
                clearButton.Click += OnClearButtonClicked;

            Window window = Window.GetWindow(this);
            if (window != null)
                window.PreviewMouseDown += OnWindowPreviewMouseDown;
        }

        #region Events
        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DropDownButton == null) throw new Exception("DropDownButton is null");
            if (Popup == null) throw new Exception("Popup is null");
            Point clickPosition = e.GetPosition(this);
            if (IsOpen && !IsMouseOver && !IsMouseOverElement(Popup, clickPosition) && !IsMouseOverElement(DropDownButton, clickPosition))
                IsOpen = false;
        }
        private void OnClearButtonClicked(object sender, RoutedEventArgs e)
        {
            foreach(var item in ItemsSource) 
                item.Unset();

            ((IListController)Controller).Filter(new(false, null));
            IsOpen = false;
            ResetDropDownButtonAppereance();
        }
        private void OnDropdownButtonClicked(object sender, RoutedEventArgs e) => IsOpen = !IsOpen;
        #endregion

        #region IsOpen
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(FilterOption), new PropertyMetadata(false));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<IFilterOption>), typeof(FilterOption), new PropertyMetadata(ItemSourceChanged));

        public IEnumerable<IFilterOption> ItemsSource
        {
            get => (IEnumerable<IFilterOption>)GetValue(ItemsSourceProperty); 
            set => SetValue(ItemsSourceProperty, value); 
        }

        private static void ItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FilterOption)d).BindEvents(e.NewValue);
        
        private void BindEvents(object new_source) 
        {
            IEnumerable<IFilterOption> source = (IEnumerable<IFilterOption>)new_source;
            foreach (IFilterOption item in source)
                item.OnSelected += Item_OnSelected;
        }

        private void Item_OnSelected(object? sender, OnSelectedEventArgs e) 
        {
            if (DropDownButton == null) throw new Exception("DropDownButton is null");
            DropDownButton.Content = ClearFilter;
            ToolTip = "Clear Filter";
            ((IListController)Controller).Filter(e);

            if (!ItemsSource.Any(s=>s.IsSelected)) ResetDropDownButtonAppereance();
        }
        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(FilterOption), new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        //private static bool IsMouseOverPopup(Popup popup, Point clickPosition)
        //{
        //    // Get the bounding box of the popup and check if the click position is inside it.
        //    var transform = popup.TransformToVisual(popup.Child);
        //    Rect popupBounds = new(transform.Transform(new Point(0, 0)), popup.Child.RenderSize);
        //    return popupBounds.Contains(clickPosition);
        //}
        private static bool IsMouseOverElement(UIElement element, Point clickPosition)
        {
            if (element == null) return false;
            Rect elementBounds = VisualTreeHelper.GetDescendantBounds(element);
            //            Point relativePosition = element.TranslatePoint(new Point(0, 0), element);
            return elementBounds.Contains(clickPosition);
        }
        protected override void OnControllerChanged(DependencyPropertyChangedEventArgs e) { }
    }
}