using Backend.Exceptions;
using Backend.Model;
using Backend.Recordsource;
using System.Data.Common;

namespace Backend.Database
{
    /// <summary>
    /// AbstractClass that defines the structure that any Database Class should use. This class implements <see cref="IAbstractDatabase"/>.
    /// </summary>
    public abstract class AbstractDatabase(ISQLModel Model) : IAbstractDatabase
    {
        public virtual string Version { get; set; } = string.Empty;
        public virtual string DatabaseName { get; set; } = string.Empty;
        public ISQLModel Model { get; set; } = Model;
        public RecordSource? Records { get; set; }
        public abstract string ConnectionString();
        public abstract DbConnection Connect();
        public Task<DbConnection> ConnectAsync() => Task.FromResult(Connect());
        private void SetCommand(DbCommand cmd, string sql, List<QueryParameter>? parameters)
        {
            if (Model == null) throw new NoModelException();
            cmd.CommandText = sql;
            if (parameters == null)
            {
                parameters = [];
                Model.SetParameters(parameters);
            }

            SetParameters(cmd, parameters);
        }

        public async IAsyncEnumerable<ISQLModel> RetrieveAsync(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (Model == null) throw new NoModelException();

            if (string.IsNullOrEmpty(sql))
                sql = Model.SelectQry;

            sql += ";";
            using (var connection = await ConnectAsync())
            {
                await connection.OpenAsync();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    SetCommand(cmd, sql, parameters);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            yield return Model.Read(reader);
                    }
                }
            }
        }

        public IEnumerable<ISQLModel> Retrieve(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (Model == null) throw new NoModelException();

            if (string.IsNullOrEmpty(sql))
                sql = Model.SelectQry;
            sql += ";";
            using (var connection = Connect())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        SetCommand(cmd, sql, parameters);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                yield return Model.Read(reader);
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public void Crud(CRUD crud, string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (Model == null) throw new NoModelException();

            if (crud == CRUD.INSERT || crud == CRUD.UPDATE)
                if (!Model.AllowUpdate()) throw new Exception("Conditions for update not met.");

            long? lastID = null;
            if (string.IsNullOrEmpty(sql))
            {
                switch (crud)
                {
                    case CRUD.INSERT:
                        sql = Model.InsertQry;
                        break;
                    case CRUD.UPDATE:
                        sql = Model.UpdateQry;
                        break;
                    case CRUD.DELETE:
                        sql = Model.DeleteQry;
                        break;
                }
            }

            using (var connection = Connect())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        SetCommand(cmd, sql!, parameters);
                        cmd.ExecuteNonQuery();
                        lastID = (crud == CRUD.INSERT) ? RetrieveLastInsertedID(connection, transaction, LastIDQry()) : null;
                    }
                    try
                    {
                        transaction.Commit();
                        if (lastID != null)
                            Model?.GetTablePK()?.SetValue(lastID);
                        UpdateRecords();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                        transaction?.Rollback();
                    }
                }
            }
        }
        public long? CountRecords(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (string.IsNullOrEmpty(sql) && Model != null)
                sql = Model.RecordCountQry + ";";
            if (string.IsNullOrEmpty(sql)) throw new Exception("No SQL Statement provided");
            return (long?)AggregateQuery(sql, parameters);
        }
        public object? AggregateQuery(string sql, List<QueryParameter>? parameters = null)
        {
            object? value = null;
            using (DbConnection connection = Connect())
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    using (DbCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Transaction = transaction;
                        SetParameters(cmd, parameters);
                        value = cmd.ExecuteScalar();
                    }
                    try
                    {
                        transaction.Commit();
                        return value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                        return value;
                    }
                }
            }
        }
        private static long? RetrieveLastInsertedID(DbConnection connection, DbTransaction transaction, string sql)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Transaction = transaction;
                return (long?)cmd.ExecuteScalar();
            }
        }

        private static void SetParameters(DbCommand cmd, List<QueryParameter>? parameters)
        {
            if (parameters == null) return;

            foreach (var parameter in parameters)
            {
                DbParameter param = cmd.CreateParameter();
                param.ParameterName = $"@{parameter.Placeholder}";
                param.Value = parameter.Value;
                cmd.Parameters.Add(param);
            }
        }

        /// <summary>
        /// Override this method to return the correct statement to retrieve the last inserted id.
        /// </summary>
        /// <returns>A string representing the SQL Statement to retrieve the last inserted id.</returns>
        protected abstract string LastIDQry();

        private void UpdateRecords()
        {
            if (Records == null) return;
            if (Model.IsNewRecord()) Records.Add(Model);
            int index = Records.IndexOf(Model);
            if (index >= 0) Records[index] = Model;
        }
    }
}
