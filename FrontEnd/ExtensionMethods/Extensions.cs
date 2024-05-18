using System.Windows;

namespace FrontEnd.ExtensionMethods
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method for Window objects.
        /// This method closes the Window calling the method and opens a new one.
        /// </summary>
        /// <param name="win"></param>
        /// <param name="newWin">The new window to open.</param>
        public static void GoToWindow(this Window win, Window? newWin)
        {
            win.Hide();
            newWin?.Show();
            win.Close();
        }
    }
}
