using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public class Walkthrough : ContentControl
    {
        int Index { get; set; } = 0;
        int lastIndex => Collection.Count - 1;
        Pages Pages { get; set; }
        INotifyCollectionChanged CollectionChanged { get; set; }
        ItemCollection Collection { get; set; }
        Button PART_PreviousButton { get; set; }
        Button PART_NextButton { get; set; }
        static Walkthrough()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Walkthrough), new FrameworkPropertyMetadata(typeof(Walkthrough)));
        }

        public Walkthrough() 
        { 
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_PreviousButton = (Button)GetTemplateChild(nameof(PART_PreviousButton));
            PART_NextButton = (Button)GetTemplateChild(nameof(PART_NextButton));

            PART_NextButton.Click += OnNextButtonClicked;
            PART_PreviousButton.Click += OnPreviousButtonClicked;
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Index == 0) return;
            Index--;
            Content = Collection[Index];
        }

        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Index == lastIndex) return;
            Index++;
            Content = Collection[Index];
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (newContent is Pages pages) 
            {
                Pages = pages;
                if (Pages.Items is INotifyCollectionChanged collectionChanged) 
                {
                    Collection = Pages.Items;
                    CollectionChanged = collectionChanged;
                    CollectionChanged.CollectionChanged += OnCollectionChanged;
                }
            }
        }
        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            
            if (Collection == null) return;
            try 
            {
                Content = Collection[Index];
            }
            catch 
            {
                throw new Exception("Something went wrong");
            }
        }

    }


    public class Pages : ItemsControl
    {
      

        //private void NotifyCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var newItem in e.NewItems)
        //        {
        //            // Handle the new item added
        //            MessageBox.Show($"Item added: {newItem}");
        //        }
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach (var oldItem in e.OldItems)
        //        {
        //            // Handle the item removed
        //            MessageBox.Show($"Item removed: {oldItem}");
        //        }
        //    }
        //}

        static Pages()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pages), new FrameworkPropertyMetadata(typeof(Pages)));
        }

        public Pages() 
        { 
            
        }
       
    }
}
