using FrontEnd.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Interaction logic for Text.xaml
    /// </summary>
    public partial class Text : TextBox
    {
        private Button? ClearButton;
        private readonly Image ClearImg = new()
        {
            Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/close.png")
        };

        static Text() 
        {
                TextProperty.OverrideMetadata(
                typeof(Text),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    null,
                    null,
                    false,
                    UpdateSourceTrigger.PropertyChanged
                ));
        }
        public Text()
        {
            InitializeComponent();
            
            Binding binding = new()
            {
                Source = this,
                Path = new PropertyPath(nameof(Text)),
                Converter = new TextToVisibility()
            };

            SetBinding(PlaceholderVisibilityProperty, binding);

            MultiBinding multiBinding = new()
            {
                Converter = new MultiBindingConverter()
            };

            Binding binding2 = new()
            {
                Source = this,
                Path = new PropertyPath(nameof(Text)),
            };

            Binding binding3 = new()
            {
                Source = this,
                Path = new PropertyPath(nameof(IsKeyboardFocusWithin)),
            };

            multiBinding.Bindings.Add(binding2);
            multiBinding.Bindings.Add(binding3);
            SetBinding(ClearButtonVisibilityProperty, multiBinding);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ClearButton = (Button)GetTemplateChild("PART_clear_button");
            ClearButton.Click += ClearButton_Click;
            ClearButton.Content = ClearImg;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) => Text = string.Empty;

        #region Placeholder
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(Text), new PropertyMetadata(string.Empty));
        #endregion

        #region PlaceholderVisibility
        public Visibility PlaceholderVisibility
        {
            get => (Visibility)GetValue(PlaceholderVisibilityProperty);
            set => SetValue(PlaceholderVisibilityProperty, value);
        }

        public static readonly DependencyProperty PlaceholderVisibilityProperty =
            DependencyProperty.Register(nameof(PlaceholderVisibility), typeof(Visibility), typeof(Text), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ClearButtonVisibility
        public Visibility ClearButtonVisibility
        {
            get => (Visibility)GetValue(ClearButtonVisibilityProperty);
            set => SetValue(ClearButtonVisibilityProperty, value);
        }

        public static readonly DependencyProperty ClearButtonVisibilityProperty =
            DependencyProperty.Register(nameof(ClearButtonVisibility), typeof(Visibility), typeof(Text), new PropertyMetadata(Visibility.Visible));
        #endregion

    }

    public class TextToVisibility : IValueConverter
    {
        string? txt;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            txt = value.ToString();
            if (txt == null) return Visibility.Visible;
            
            return (txt.Length > 0) ? Visibility.Hidden : Visibility.Visible;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible) ? "" : txt;
        }
    }

    public class TextToVisibilityInvert : IValueConverter
    {
        string? txt;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            txt = value.ToString();
            if (txt == null) return Visibility.Hidden;

            return (txt.Length > 0) ?  Visibility.Visible : Visibility.Hidden;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible) ? txt : "";
        }
    }

    public class MultiBindingConverter : IMultiValueConverter
    {
        string? txt;
        bool focus;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            txt = values[0].ToString();
            focus = System.Convert.ToBoolean(values[1].ToString());
            if (txt == null) return Visibility.Hidden;
            return (txt.Length > 0 && focus) ? Visibility.Visible : Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            List<object> list = [];
            Visibility visibility = (Visibility)value;
            list.Add((visibility == Visibility.Visible) ? txt : "");
            return [.. list];
        }
    }
}
