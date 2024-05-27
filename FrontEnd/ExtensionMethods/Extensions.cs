using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.ExtensionMethods
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method for Window objects.
        /// This method closes the current Window to open a new one.
        /// </summary>
        /// <param name="win"></param>
        /// <param name="newWin">The new window to open.</param>
        public static void GoToWindow(this Window win, Window? newWin)
        {
            win.Hide();
            newWin?.Show();
            win.Close();
        }

        public static IAbstractFormController? CurrentTabController(this TabControl tabControl) 
        {
            Frame frame = (Frame) tabControl.SelectedContent;
            Page page = (Page)frame.Content;
            return page.DataContext as IAbstractFormController;
        }
    }
}
