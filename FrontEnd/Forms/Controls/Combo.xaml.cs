using Backend.Recordsource;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends <see cref="ComboBox"/> and adds some extra functionalities for dealing with the SelectedItem property.
    /// Furthermore, its ItemsSource is meant to be a <see cref="RecordSource"/> object.
    /// </summary>
    public partial class Combo : ComboBox
    {
        public Combo() => InitializeComponent();

        private RecordSource GetSource() => (RecordSource)ItemsSource;
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            var item = GetSource().FirstOrDefault(s => s.Equals(SelectedItem));
            Dispatcher.BeginInvoke(() => Text = item?.ToString());
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


    }
}
