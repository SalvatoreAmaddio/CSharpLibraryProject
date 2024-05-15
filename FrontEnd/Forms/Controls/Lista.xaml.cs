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
            DependencyProperty.Register(nameof(Header), typeof(Grid), typeof(Lista), new PropertyMetadata(OnHeaderPropertyChanged));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary resourceDict = new ()
            {
                Source = new Uri("pack://application:,,,/FrontEnd;component/Themes/controls.xaml")
            };
            Style? labelStyle = null;

            if (resourceDict["ColumnStyle"] is Style columnStyle)
                labelStyle = new Style(targetType: typeof(Label), basedOn: columnStyle);

            Grid grid = (Grid)e.NewValue;
            grid.Resources.MergedDictionaries.Add(resourceDict);
            grid.Resources.Add(typeof(Label), CreateStyle(labelStyle));
            grid.Name = "listHeader";
        }
        #endregion
        
        private static Style CreateStyle(Style? basedOn) 
        {
            return new Style(targetType: typeof(Label), basedOn: basedOn);
        }

        private IAbstractFormListController? Controller => (IAbstractFormListController)DataContext;

        public Lista() => InitializeComponent();

        private object? OldSelection;
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            int lastRemovedIndex = e.RemovedItems.Count - 1;
            if (lastRemovedIndex >= 0) 
            {
                OldSelection = e.RemovedItems[lastRemovedIndex];
            }
            int lastIndex  = e.AddedItems.Count - 1;
            try 
            {
                AbstractModel? lastSelectedObject = (AbstractModel?)e.AddedItems[lastIndex];
                ScrollIntoView(lastSelectedObject);
            }
            catch (Exception) { }
        }

        private void OnListViewItemLostFocus(object sender, RoutedEventArgs e)
        {
            if (((ListViewItem)sender).DataContext is not AbstractModel record) return;
           
            if (record.IsNewRecord()) 
            {
                MessageBoxResult result = MessageBox.Show("You must save the record before performing any other action. Do you want to save the record?","Wait",MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    bool? updateResult = Controller?.PerformUpdate();
                    if (!updateResult!.Value) //if the update failed, move the focus to the ListViewItem.
                    {
                        ((ListViewItem)sender).Focus();
                        ScrollIntoView(sender);
                    }
                }
                else //rollback to the previous selecteditem.
                {
                    ISQLModel? oldModel = (ISQLModel?)OldSelection;
                    Controller?.CleanSource();
                    Controller?.GoAt(oldModel);
                }
            }
        }

        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e) 
        {
            if (((ListViewItem)sender).DataContext is not AbstractModel record) return;
            Controller?.GoAt(record);
        }
    }
}