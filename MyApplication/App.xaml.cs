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
            CredentialManager.Delete("EncrypterKey");
            CredentialManager.Delete("EncrypterIV");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Sys.LoadEmbeddedDll("System.Data.SQLite");
            try 
            {
                DatabaseManager.Do.Add(new SQLiteDatabase(new Employee()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new Gender()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new Department()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new JobTitle()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new Payslip()));
                DatabaseManager.Do.Add(new SQLiteDatabase(new User()));
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
