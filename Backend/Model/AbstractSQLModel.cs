﻿using Backend.Database;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Backend.Model
{

    /// <summary>
    /// This class defines the basic structure that each Model should extend.
    /// </summary>
    abstract public class AbstractSQLModel : ISQLModel
    {
        public string SelectQry { get; set; } = string.Empty;
        public string UpdateQry { get; set; } = string.Empty;
        public string InsertQry { get; set; } = string.Empty;
        public string DeleteQry { get; set; } = string.Empty;
        public string RecordCountQry { get; set; } = string.Empty;

        public AbstractSQLModel()
        {
            _ = new QueryBuilder(this);
        }

        public abstract ISQLModel Read(DbDataReader reader);
        public IEnumerable<ITableField> GetTableFields() => GetTableFieldsAs<Field>();
        public TableField? GetTablePK() 
        {
            PropertyInfo? prop = GetType().GetProperties().Where(s=>s.GetCustomAttribute<PK>()!=null).FirstOrDefault();
            if (prop == null) return null;
            AbstractField? field = prop.GetCustomAttribute<PK>();
            return (field != null) ? new TableField(field, prop, this) : null;
        }

        public IEnumerable<ITableField> GetTableFKs() => GetTableFieldsAs<FK>();

        public string GetTableName()
        {
            Table? tableAttr = GetType().GetCustomAttribute<Table>();
            return $"{tableAttr}";
        }

        private IEnumerable<PropertyInfo> GetMandatoryFields() 
        {
            Type type = GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (PropertyInfo prop in props)
            {
                var field = prop.GetCustomAttribute<Mandatory>();
                if (field != null)
                    yield return prop;
            }
        }

        private IEnumerable<ITableField> GetTableFieldsAs<F>() where F : AbstractField
        {
            Type type = GetType();
            PropertyInfo[] props = type.GetProperties();
            bool isForeignKey = typeof(F) == typeof(FK);
            foreach (PropertyInfo prop in props)
            {
                AbstractField? field = prop.GetCustomAttribute<F>();
                if (field != null) 
                {
                    if (isForeignKey) yield return new FKField(field, prop, this);
                    else yield return new TableField(field, prop, this);
                }
            }
        }

        public bool IsNewRecord() => (long?)GetTablePK()?.GetValue() == 0;
        
        public override bool Equals(object? obj)
        {
            if (obj is not AbstractSQLModel other) return false;
            long? value = (long?)(GetTablePK()?.GetValue());
            long? value2 = (long?)(other?.GetTablePK()?.GetValue());
            if (value == null) return false;
            return value == value2;
        }

        public override int GetHashCode() => HashCode.Combine(GetTablePK()?.GetValue());

        public virtual void SetParameters(List<QueryParameter>? parameters) 
        {
            parameters?.Add(new(GetTablePK()?.Name!, GetTablePK()?.GetValue()));

            foreach(ITableField field in GetTableFields()) 
                parameters?.Add(new(field.Name, field.GetValue()));

            foreach (ITableField field in GetTableFKs()) 
            {
                IFKField fk_field = (IFKField)field;
                parameters?.Add(new(fk_field.Name, fk_field.PK?.GetValue()));
            }
        }

        private readonly List<EmptyField> emptyFields = [];

        public string GetEmptyMandatoryFields() 
        {
            StringBuilder sb = new();

            foreach(EmptyField field in emptyFields) 
            { 
                sb.Append($"- {field.Name}\n");
            }

            return sb.ToString();
        }

        public virtual bool AllowUpdate()
        {
            emptyFields.Clear();

            foreach(var field in GetMandatoryFields()) 
            {
                string name = field.Name;
                object? value = field.GetValue(this);

                if (value == null) 
                {
                    emptyFields.Add(new(name,value));
                    continue;
                }

                if (field.PropertyType == typeof(string)) 
                    if (string.IsNullOrEmpty(value.ToString())) 
                    {
                        emptyFields.Add(new(name, value));
                        continue;
                    }

                if (field.PropertyType == typeof(ISQLModel))
                    if (((ISQLModel)field).IsNewRecord()) 
                    {
                        emptyFields.Add(new(name, value));
                        continue;
                    }
            }

            return emptyFields.Count == 0;
        }

        internal class EmptyField(string name, object? value) 
        {
            public string Name { get; } = name;
            public object? Value { get; } = value;
            public override string? ToString() => Name;
        }
    }
}
