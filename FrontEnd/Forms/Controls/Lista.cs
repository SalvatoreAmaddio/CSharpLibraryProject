using FrontEnd.Controller;
using FrontEnd.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FrontEnd.Forms
{
    public class Lista : ListView
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

        private static Style CreateStyle(Style? basedOn)
        {
            return new Style(targetType: typeof(Label), basedOn: basedOn);
        }
        private IAbstractFormListController? Controller => (IAbstractFormListController)DataContext;

        ResourceDictionary resourceDict = new()
        {
            Source = new Uri("pack://application:,,,/FrontEnd;component/Themes/ListaStyle.xaml")
        };

        private object? OldSelection;

        public Lista()
        {
            Style listaItem = (Style)resourceDict["ListaItemStyle"];
            listaItem.Setters.Add(new EventSetter
            {
                Event = ListViewItem.GotFocusEvent,
                Handler = new RoutedEventHandler(OnListViewItemGotFocus)
            });

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
            if (e.NewFocus is null || e.NewFocus is not FrameworkElement element) return;
            if (element.DataContext is IAbstractFormController) 
            {
                AbstractModel ListViewItemDataContext = (AbstractModel)((ListViewItem)sender).DataContext;
                OnListViewItemLostFocus(ListViewItemDataContext);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            int lastRemovedIndex = e.RemovedItems.Count - 1;

            if (lastRemovedIndex >= 0) 
            {
                OldSelection = e.RemovedItems[lastRemovedIndex];
            }

            int lastIndex = e.AddedItems.Count - 1;
            try
            {
                AbstractModel? lastSelectedObject = (AbstractModel?)e.AddedItems[lastIndex];
                ScrollIntoView(lastSelectedObject);
            }
            catch (Exception) { }
        }
        
        private void OnListViewItemLostFocus(AbstractModel record)
        {

            if (record.IsNewRecord())
            {
                MessageBoxResult result = MessageBox.Show("You must save the record before performing any other action. Do you want to save the record?", "Wait", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    bool? updateResult = Controller?.PerformUpdate();
                    if (!updateResult!.Value) //if the update failed, move the focus to the ListViewItem.
                    {
                        ScrollIntoView(record);
                    }
                }
                else //rollback to the previous selecteditem.
                {
                    AbstractModel? oldModel = (AbstractModel?)OldSelection;
                    Controller?.CleanSource();
                    Controller?.GoAt(oldModel);
                }
            }
        }

        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e)
        {
            if (((ListViewItem)sender).DataContext is not AbstractModel record) return;
            if (!record.Equals(SelectedItem)) 
            {
                OnListViewItemLostFocus((AbstractModel)SelectedItem);
            }
            Controller?.GoAt(record);
        }
    }
}
