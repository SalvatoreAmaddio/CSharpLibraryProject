using FrontEnd.Model;
using FrontEnd.Utils;
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
        private readonly ResourceDictionary resourceDict = Helper.GetDictionary(nameof(Combo));

        public Combo() 
        { 
            ItemContainerStyle = (Style)resourceDict["ComboItemContainerStyle"];
            Style = (Style)resourceDict["ComboStyle"];
        }

        public AbstractModel? ParentModel => DataContext as AbstractModel;
        
        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            RequerySource();
        }

        private void RequerySource() 
        {
            string tempControllerRecordSource = ControllerRecordSource;
            object tempSelectedItem = SelectedItem;
            bool? tempIsDirty = ParentModel?.IsDirty;
            ClearValue(ControllerRecordSourceProperty);
            SetBinding(ItemsSourceProperty, new Binding($"{nameof(DataContext)}.{tempControllerRecordSource}")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
            });
            SelectedItem = tempSelectedItem;
            if (tempSelectedItem!=null && tempIsDirty.HasValue)
                ((AbstractModel)DataContext).IsDirty = tempIsDirty.Value;
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

        public static readonly DependencyProperty ControllerRecordSourceProperty = DependencyProperty.Register(nameof(ControllerRecordSource), typeof(string), typeof(Combo), new PropertyMetadata(string.Empty, OnControllerRecordSourcePropertyChanged));
        private static void OnControllerRecordSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isEmpty = string.IsNullOrEmpty(e.NewValue.ToString());
            Combo control = (Combo)d;
            if (!isEmpty)
                control.SetBinding(ItemsSourceProperty, new Binding($"{nameof(DataContext)}.{e.NewValue}")
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                    });
            else control.ClearValue(ItemsSourceProperty);
        }
        #endregion

    }
}