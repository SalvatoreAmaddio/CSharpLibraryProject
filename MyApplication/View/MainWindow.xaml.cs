using FrontEnd.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IEnumerable<TabItem> tabWithControllers = MainTab.Items.Cast<TabItem>().Where(item => item.Content is Frame frame && frame.Content is FrameworkElement element && element.DataContext is IAbstractFormController);
            foreach (TabItem item in tabWithControllers)
            {
                FrameworkElement element = (FrameworkElement)((Frame)item.Content).Content;
                IAbstractFormController controller = (IAbstractFormController)element.DataContext;
                controller.OnWindowClosing(sender, e);
                if (e.Cancel) 
                {
                        MainTab.SelectedItem = item;
                        break;
                }
            }
        }
    }
}
