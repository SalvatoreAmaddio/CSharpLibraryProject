using Backend.Controller;
using System.Windows;
using System.Windows.Controls;
using Backend.Model;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends the <see cref="ListView"/> class and adds extra functionalities.
    /// Such as column's header, see the <see cref="Header"/> property.
    /// Also, the DataContext of this object is meant to be a <see cref="IAbstractSQLModelController"/>.
    /// <para/>
    /// Its ItemsSource property should be a IEnumerable&lt;<see cref="ISQLModel"/>&gt; such as a <see cref="Backend.Recordsource.RecordSource"/>
    /// </summary>
    public partial class Lista : ListView
    {
        #region Header
        /// <summary>
        /// Gets and Sets a <see cref="Grid"/> object which serves as column's header.
        /// </summary>
        public Grid Header
        {
            get => (Grid)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(Grid), typeof(Lista), new PropertyMetadata());
        #endregion

        private IAbstractSQLModelController? Controller { get; set; }

        public Lista() => InitializeComponent();

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            Controller = (IAbstractSQLModelController)DataContext;
            base.OnSelectionChanged(e);
            int lastIndex  = e.AddedItems.Count - 1;
            try 
            {
                ISQLModel? lastSelectedObject = (ISQLModel?)e.AddedItems[lastIndex];
                Controller?.GoAt(lastSelectedObject);
            }
            catch (Exception) { }
        }

        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e) => SelectedItem = ((ListViewItem)sender).DataContext;
    }
}