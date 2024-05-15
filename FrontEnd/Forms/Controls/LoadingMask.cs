using Backend.Database;
using FrontEnd.Utils;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms.Controls
{
    /// <summary>
    /// This class instantiate the content for Window object to show a loading process
    /// </summary>
    public class LoadingMask : ContentControl
    {
        static LoadingMask() => DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingMask), new FrameworkPropertyMetadata(typeof(LoadingMask)));

        /// <summary>
        /// Sets the name of the Window to open once the loading process has completed. <para/>
        /// <c>IMPORTANT:</c> the Window to open must be in a folder named 'View'.
        /// </summary>
        public string MainWindow
        {
            private get => (string)GetValue(MainWindowProperty);
            set => SetValue(MainWindowProperty, value);
        }

        public static readonly DependencyProperty MainWindowProperty = DependencyProperty.Register(nameof(MainWindow), typeof(string), typeof(LoadingMask), new PropertyMetadata(string.Empty));

        public LoadingMask() => Loaded += OnLoading;
        
        protected virtual async void OnLoading(object sender, RoutedEventArgs e)
        {
            string? assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
            string? Namespace = Assembly.GetEntryAssembly()?.EntryPoint?.DeclaringType?.Namespace; 
            Type? mainWinType = Type.GetType($"{Namespace}.View.{MainWindow}, {assemblyName}") ?? throw new Exception($"Could not find the Type of {MainWindow}");
            Window? window = Helper.GetActiveWindow();
            await Task.Run(DatabaseManager.Do.FetchData);
            window?.Hide();
            var m = (Window?)Activator.CreateInstance(mainWinType);
            m?.Show();
            window?.Close();
        }
    }
}
