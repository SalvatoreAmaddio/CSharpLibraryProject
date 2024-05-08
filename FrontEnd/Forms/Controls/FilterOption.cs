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
            DropDownButton = (Button)GetTemplateChild("PART_dropdown_button");
            DropDownButton.Click += OnDropdownButtonClicked;
            ResetDropDownButtonAppereance();

            if (GetTemplateChild("PART_clear_button") is Button clearButton)
                clearButton.Click += OnClearButtonClicked;
        }

        #region Events
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

        protected override void OnControllerChanged(DependencyPropertyChangedEventArgs e) { }
    }
}