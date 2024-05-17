using FrontEnd.Controller;
using FrontEnd.Model;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends the <see cref="ListView"/> class and adds extra functionalities.
    /// Such as column's header, see the <see cref="Header"/> property.
    /// Also, the DataContext of this object is meant to be a <see cref="IAbstractFormListController"/>.
    /// <para/>
    /// Its ItemsSource property should be a IEnumerable&lt;<see cref="AbstractModel"/>&gt; such as a <see cref="Backend.Source.RecordSource"/>
    /// </summary>
    public class Lista : ListView
    {
        bool skipFocusEvent = false;

        #region Header
        /// <summary>
        /// Gets and Sets a <see cref="Grid"/> object which serves as column's header.
        /// </summary>
        public Grid Header
        {
            get => (Grid)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(Grid), typeof(Lista), new PropertyMetadata(OnHeaderPropertyChanged));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary resourceDict = new()
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

        private static Style CreateStyle(Style? basedOn) => new(targetType: typeof(Label), basedOn: basedOn);
        private IAbstractFormListController? Controller => (IAbstractFormListController)DataContext;

        private readonly ResourceDictionary resourceDict = Helper.GetDictionary(nameof(Lista));
        private readonly Style listaItem;

        private object? OldSelection;
        private readonly EventSetter ListViewItemGotFocusEventSetter = new()
        {
            Event = ListViewItem.GotFocusEvent,
        };

        public Lista()
        {
            ListViewItemGotFocusEventSetter.Handler = new RoutedEventHandler(OnListViewItemGotFocus);
            listaItem = (Style)resourceDict["ListaItemStyle"];
            listaItem.Setters.Add(ListViewItemGotFocusEventSetter);
            listaItem.Setters.Add(new EventSetter
            {
                Event = ListViewItem.LostKeyboardFocusEvent,
                Handler = new KeyboardFocusChangedEventHandler(ListViewItemKeyboardFocusChanged)
            });

            ItemContainerStyle = listaItem;
            Style = (Style)resourceDict["ListaStyle"];
        }

        private void ListViewItemKeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus is not FrameworkElement element || e.OldFocus is AbstractButton) return;
            if (element.DataContext is IAbstractFormController && element is not AbstractButton)
            {
                AbstractModel ListViewItemDataContext = (AbstractModel)((ListViewItem)sender).DataContext;
                OnListViewItemLostFocus(ListViewItemDataContext);
            }
            e.Handled = true;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            int lastRemovedIndex = e.RemovedItems.Count - 1;

            if (lastRemovedIndex >= 0)
                OldSelection = e.RemovedItems[lastRemovedIndex];

            int lastIndex = e.AddedItems.Count - 1;
            try
            {
                AbstractModel? lastSelectedObject = (AbstractModel?)e.AddedItems[lastIndex];
                ScrollIntoView(lastSelectedObject);
            }
            catch { }
        }
        
        private bool OnListViewItemLostFocus(AbstractModel? record)
        {
            if (record is null) return true;
            if (!record.IsDirty && !record.IsNewRecord()) return true;

            MessageBoxResult result = MessageBox.Show("You must save the record before performing any other action. Do you want to save the record?", "Wait", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                bool? updateResult = Controller?.PerformUpdate();
                if (!updateResult!.Value) //if the update failed, move the focus to the ListViewItem.
                    ScrollIntoView(record);
            }
            else //rollback to the previous selecteditem.
            {
                if (record.IsDirty && !record.IsNewRecord()) //you are updating a record which is not new
                { 
                    Refocus();
                    return false;
                }
                AbstractModel? oldModel = (AbstractModel?)OldSelection;
                Controller?.CleanSource();
                Controller?.GoAt(oldModel);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reset the focus to the current selected ListViewItem by passing the <see cref="OnListViewItemGotFocus(object, RoutedEventArgs)"/>
        /// </summary>
        private void Refocus() 
        {
            ListViewItem listViewItem = (ListViewItem)ItemContainerGenerator.ContainerFromItem(SelectedItem);
            skipFocusEvent = true;
            listViewItem.Focus();
            skipFocusEvent = false;
        }

        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e)
        {
            if (skipFocusEvent) return;
            if (((ListViewItem)sender).DataContext is not AbstractModel record) return;
            if (!record.Equals(SelectedItem)) 
            {
                AbstractModel model = (AbstractModel)SelectedItem;
                bool result = OnListViewItemLostFocus((AbstractModel)SelectedItem);
                if (!result) return;
                if (!model.AllowUpdate()) 
                {
                    ListViewItem listViewItem = (ListViewItem)ItemContainerGenerator.ContainerFromItem(SelectedItem);
                    e.Handled = true;
                    skipFocusEvent = true;
                    listViewItem.Focus();
                    skipFocusEvent = false;
                    return;
                }
                Controller?.GoAt(record);
            }
        }
    }
}