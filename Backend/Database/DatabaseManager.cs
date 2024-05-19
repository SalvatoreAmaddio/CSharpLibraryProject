using Backend.Model;
using Backend.Source;
namespace Backend.Database
{
    /// <summary>
    /// This class follows the Singleton pattern. 
    /// It is a collections of List&lt;ISQLModel> that can be used accross the application.
    /// </summary>
    public sealed class DatabaseManager
    {
        private static readonly Lazy<DatabaseManager> lazyInstance = new(() => new DatabaseManager());
        private readonly List<IAbstractDatabase> Databases;
        public static DatabaseManager Do => lazyInstance.Value;

        /// <summary>
        /// The number of IAbstractDatabases.
        /// </summary>
        /// <value>An Integer.</value>
        public int Count => Databases.Count;   
        private DatabaseManager() => Databases = [];

        /// <summary>
        /// It adds a database object.
        /// </summary>
        /// <param name="db">An object implementing <see cref="IAbstractDatabase"/></param>
        public void Add(IAbstractDatabase db) => Databases.Add(db);

        /// <summary>
        /// For each database, it calls concurrently, the <see cref="IAbstractDatabase.RetrieveAsync(string?, List{QueryParameter}?)"/>.
        /// <para/>
        /// Then, it awaits for all tasks to complete and sets for each Database their <see cref="IAbstractDatabase.Records"/> property. 
        /// You can access the Database by calling <see cref="Do"/>
        /// <para/>
        /// For Example:
        /// <code>
        /// await MainDB.Do.FetchData();
        /// RecordSource source = DatabaseManager.Do[0].Records; //get the RecordSource of the first Database;
        /// </code>
        /// </summary>
        /// <returns>A Task</returns>
        public async Task FetchData() 
        {
            List<Task<List<ISQLModel>>> tasks = [];

            foreach (IAbstractDatabase db in Databases)
            {
                Task<List<ISQLModel>> task = db.RetrieveAsync().ToListAsync().AsTask();
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            for(int i = 0; i < tasks.Count; i++) this[i].Records = new(tasks[i].Result);
        }

        /// <summary>
        /// Gets a Database based on its zero-based position index.
        /// <para/>
        /// For Example:
        /// <code>
        /// IAbstractDatabase db = DatabaseManager.Do[0]; //get the first IAbstractDatabase;
        /// </code>
        /// </summary>
        /// <param name="index">zero-based position.</param>
        /// <returns>An IAbstractDatabase</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public IAbstractDatabase this[int index] => (index < 0 || index >= Databases.Count) ? throw new IndexOutOfRangeException() : Databases[index];

        public IAbstractDatabase? Find(string name) 
        { 
            foreach(IAbstractDatabase db in Databases) 
            {
                if (db.Model.GetType().Name.Equals(name)) return db;
            }       
            return null;
        }
    }

}
