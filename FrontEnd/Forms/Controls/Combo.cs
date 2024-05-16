using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends <see cref="ComboBox"/> and adds some extra functionalities for dealing with the SelectedItem property.
    /// Furthermore, its ItemsSource is meant to be a <see cref="Backend.Source.RecordSource"/> object.
    /// </summary>
    public partial class Combo : ComboBox
    {
        private readonly ResourceDictionary resourceDict = new()
        {
            Source = new Uri("pack://application:,,,/FrontEnd;component/Themes/Combo.xaml")
        };

        public Combo() 
        { 
            ItemContainerStyle = (Style)resourceDict["ComboItemContainerStyle"];
            Style = (Style)resourceDict["ComboStyle"];
        }

        protected override async void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            try
            {
                object? model = e.AddedItems[e.AddedItems.Count - 1];
                object? item = null;
                foreach (object record in ItemsSource)
                {
                    if (record.Equals(model))
                    {
                        item = record;
                        break;
                    }
                }
                await FillText(item);
            }
            catch { }
        }

        private Task FillText(object? item) 
        {
            Text = item?.ToString();
            return Task.CompletedTask;
        }

        #region Placeholder
        /// <summary>
        /// Gets and sets the Placeholder
        /// </summary>
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(Combo), new PropertyMetadata(string.Empty));
        #endregion

        #region ControllerSource
        /// <summary>
        /// This property works as a short-hand to set a Relative Source Binding between the combo's ItemSource and a <see cref="Lista"/>'s DataContext's IEnumerable Property.
        /// </summary>
        public string ControllerRecordSource
        {
            private get => (string)GetValue(ControllerRecordSourceProperty);
            set => SetValue(ControllerRecordSourceProperty, value);
        }

        public static readonly DependencyProperty ControllerRecordSourceProperty = DependencyProperty.Register(nameof(ControllerRecordSource), typeof(string), typeof(Combo), new PropertyMetadata(string.Empty, OnIsWithinListPropertyChanged));
        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isEmpty = string.IsNullOrEmpty(e.NewValue.ToString());
            if (!isEmpty)
                ((Combo)d).SetBinding(ItemsSourceProperty, new Binding($"{nameof(DataContext)}.{e.NewValue}")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            else BindingOperations.ClearBinding(d, ItemsSourceProperty);
        }
        #endregion

    }
}