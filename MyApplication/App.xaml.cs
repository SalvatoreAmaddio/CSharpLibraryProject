using Backend.Database;
using MyApplication.Model;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MyApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() 
        {
            DatabaseManager.Do.Add(new SQLiteDatabase(new Person()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new Gender()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new Department()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new JobTitle()));
        }
        //protected override async void OnStartup(StartupEventArgs e)
        //{
        //    DatabaseManager.Do.Add(new SQLiteDatabase(new Person()));            
        //    base.OnStartup(e);
        //    await DatabaseManager.Do.FetchData();
        //}
    }

}
