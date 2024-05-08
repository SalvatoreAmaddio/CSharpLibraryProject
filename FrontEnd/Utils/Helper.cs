using System.Windows.Media.Imaging;

namespace FrontEnd.Utils
{
    public class Helper
    {
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
