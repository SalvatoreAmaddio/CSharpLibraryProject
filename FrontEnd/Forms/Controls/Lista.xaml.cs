using Backend.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void ListViewItem_GotFocus(object sender, RoutedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            SelectedItem = item.DataContext;
        }
    }
}