using Backend.Database;
using Backend.Utils;
using FrontEnd.Model;
using MyApplication.Model;
using System.Windows;

namespace MyApplication
{
    public partial class App : Application
    {
        public App() 
        {
            Sys.LoadAll();
            DatabaseManager.Do.Add(new SQLiteDatabase(new Employee()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new Gender()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new Department()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new JobTitle()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new Payslip()));
            DatabaseManager.Do.Add(new SQLiteDatabase(new User()));
        }
    }

}
