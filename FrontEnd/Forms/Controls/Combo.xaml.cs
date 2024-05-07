using Backend.Recordsource;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends <see cref="ComboBox"/> and adds some extra functionalities for dealing with the SelectedItem property.
    /// Furthermore, the ItemsSource for this object is meant to be a <see cref="RecordSource"/> object.
    /// </summary>
    public partial class Combo : ComboBox
    {
        public Combo() => InitializeComponent();


        /// <summary>
        /// Gets the ItemsSource as a RecordSource.
        /// </summary>
        /// <returns>A RecordSource</returns>
        private RecordSource GetSource() => (RecordSource)ItemsSource;
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            var item = GetSource().FirstOrDefault(s => s.Equals(SelectedItem));
            Dispatcher.BeginInvoke(() => Text = item?.ToString());
        }

        #region Placeholder
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
