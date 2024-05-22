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
            Sys.LoadAllEmbeddedDll();
            DatabaseManager.Add(new SQLiteDatabase(new Employee()));
            DatabaseManager.Add(new SQLiteDatabase(new Gender()));
            DatabaseManager.Add(new SQLiteDatabase(new Department()));
            DatabaseManager.Add(new SQLiteDatabase(new JobTitle()));
            DatabaseManager.Add(new SQLiteDatabase(new Payslip()));
            DatabaseManager.Add(new SQLiteDatabase(new User()));
        }
    }

}
