using Backend.Controller;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Interaction logic for Lista.xaml
    /// </summary>
    public partial class Lista : ListView
    {
        #region Header
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
                object? lastSelectedObject = e.AddedItems[lastIndex];
                Controller?.GoAt((Backend.Model.ISQLModel?)lastSelectedObject);
            }
            catch (Exception)
            {

            }
        }

        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            SelectedItem = item.DataContext;
        }
    }
}