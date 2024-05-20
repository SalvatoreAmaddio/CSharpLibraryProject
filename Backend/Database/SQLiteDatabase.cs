using Backend.Model;
using Backend.Utils;
using System.Data.Common;
using System.Data.SQLite;
using System.Data;
using System.Reflection;

namespace Backend.Database
{

    /// <summary>
    /// This class is a concrete definition of <see cref="AbstractDatabase"/> meant for dealing with a SQLite database.
    /// </summary>
    public class SQLiteDatabase(ISQLModel Model) : AbstractDatabase(Model)
    {
        public override string Version { get; set; } = "3";
        public override string DatabaseName { get; set; } = "Data/mydb.db";
        public override string ConnectionString() => $"Data Source={DatabaseName};Version={Version};";
        protected override string LastIDQry() => "SELECT last_insert_rowid()";

        /// <summary>
        /// If the software is published as SingleFile, the SQLite connection must be established through external Assembly.
        /// On Application' startup, it is important that you call <see cref="Sys.LoadEmbeddedDll(string)"/> by passing the string "System.Data.SQLite" as its argument.
        /// </summary>
        /// <returns>A DbConnection object</returns>
        /// <exception cref="Exception">Throw an exception</exception>
        public override DbConnection Connect()
        {
            Assembly? assembly = Sys.LoadedDLL.FirstOrDefault(s => s.Name.Equals("System.Data.SQLite"))?.Assembly;
            if (assembly == null) return new SQLiteConnection(ConnectionString()); //if the assembly is null, it is assumed the application is not published as single file.
            //Attempt to create an instance of IDBConnection through the assembly.            
            IDbConnection ? connection = (IDbConnection?)assembly.CreateInstance("System.Data.SQLite.SQLiteConnection") ?? throw new Exception("Failed to create a connection object from LoadedAssembly");
            connection.ConnectionString = ConnectionString(); //set the connection string.
            return (DbConnection)connection;
        }
    }
}