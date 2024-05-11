using Backend.Database;
using MyApplication.Model;
using System.Windows;

namespace MyApplication
{
    public partial class App : Application
    {
        public App() 
        {
            DatabaseManager.Do.Add(new SQLiteDatabase(new Employee()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new Gender()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new Department()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new JobTitle()));
        }
    }

}
