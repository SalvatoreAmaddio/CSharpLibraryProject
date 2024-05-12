using System.Windows;
using System.Windows.Controls;
using FrontEnd.Controller;
using FrontEnd.Model;
using Backend.Model;
using Backend.Recordsource;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends the <see cref="ListView"/> class and adds extra functionalities.
    /// Such as column's header, see the <see cref="Header"/> property.
    /// Also, the DataContext of this object is meant to be a <see cref="IAbstractFormController"/>.
    /// <para/>
    /// Its ItemsSource property should be a IEnumerable&lt;<see cref="ISQLModel"/>&gt; such as a <see cref="RecordSource"/>
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

        private IAbstractFormController? Controller { get; set; }

        public Lista() => InitializeComponent();

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            Controller = (IAbstractFormController)DataContext;
            base.OnSelectionChanged(e);
            int lastIndex  = e.AddedItems.Count - 1;
            try 
            {
                AbstractModel? lastSelectedObject = (AbstractModel?)e.AddedItems[lastIndex];
                if (lastSelectedObject != null && lastSelectedObject.IsNewRecord()) return;
                Controller?.GoAt(lastSelectedObject);
            }
            catch (Exception) { }
        }

        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e) => SelectedItem = ((ListViewItem)sender).DataContext;
    }
}