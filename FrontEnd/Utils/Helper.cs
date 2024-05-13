using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FrontEnd.Utils
{
    public class Helper
    {
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

        public static BitmapImage LoadImg(string path) 
        {
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
