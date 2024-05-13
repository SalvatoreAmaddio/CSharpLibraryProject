using Backend.Database;
using Backend.Utils;
using MyApplication.Model;
using System.Windows;

namespace MyApplication
{
    public partial class App : Application
    {
        public App() 
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Sys.LoadEmbeddedDll("SQLite.Interop.dll");
            try 
            {
                DatabaseManager.Do.Add(new SQLiteDatabase(new Employee()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new Gender()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new Department()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new JobTitle()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new Payslip()));
            }
            catch(Exception ex)  
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).Message);
        }
    }

}
