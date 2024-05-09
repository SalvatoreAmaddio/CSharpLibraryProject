using FrontEnd.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends a <see cref="TextBox"/> and adds extra functionalities.
    /// Such as a Placeholder property.
    /// <para/>
    /// It also overrides the TextProperty whose Binding is set to <see cref="UpdateSourceTrigger.PropertyChanged"/>.
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
            ClearButton.Click += OnClearButtonClicked;
            ClearButton.Content = ClearImg;
        }

        private void OnClearButtonClicked(object sender, RoutedEventArgs e) => Text = string.Empty;

        #region Placeholder
        /// <summary>
        /// Gets and Sets the Placeholder to be displayed when the TextBox is empty.
        /// </summary>
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

    /// <summary>
    /// This class converts the value of a <see cref="TextBox.Text"/> property to a <see cref="Visibility"/> object.
    /// </summary>
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
