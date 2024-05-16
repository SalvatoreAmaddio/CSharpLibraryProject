using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FrontEnd.Utils
{
    public class Helper
    {
        /// <summary>
        /// Load a BitmapImage from the Images.xaml dictionary.
        /// </summary>
        /// <param name="imgKey">The resource's key</param>
        /// <returns>A BitmapImage</returns>
        public static BitmapImage LoadFromImages(string imgKey) =>
        LoadImg(GetDictionary("Images")[imgKey]?.ToString());

        /// <summary>
        /// Gets a dictionary from the Themes directory.
        /// </summary>
        /// <param name="name">The name of the dictionary</param>
        /// <returns>A ResourceDictionary</returns>
        public static ResourceDictionary GetDictionary(string name) => 
        new() 
        {
            Source = new Uri($"pack://application:,,,/FrontEnd;component/Themes/{name}.xaml")
        };

        public static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            if (current == null) return null;
            do
            {
                if (current is T)
                    return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        /// <summary>
        /// Gets the active window.
        /// </summary>
        /// <returns>A Window</returns>
        public static Window? GetActiveWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                    return window;
            }
            return null;
        }

        public static T? FindFirstChildOfType<T>(DependencyObject? parent) where T : DependencyObject
        {
            if (parent == null) return null;
            if (parent is T)
                return parent as T;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T? result = FindFirstChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Loads a BitmapImage which can be used as a Source in a Image Control.
        /// </summary>
        /// <param name="path">The path to the image</param>
        /// <returns>A BitmapImage</returns>
        /// <exception cref="Exception">Path cannot be null</exception>
        public static BitmapImage LoadImg(string? path) 
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("Path cannot be empty");
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
