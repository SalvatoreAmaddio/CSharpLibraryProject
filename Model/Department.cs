using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace MyApplication.Model
{
    [Table(nameof(Department))]
    public class Department : AbstractModel
    {
        long _departmentid;
        string _departmentname = string.Empty;

        [PK]
        public long DepartmentID { get => _departmentid; set => UpdateProperty(ref value, ref _departmentid); }
        [Field]
        public string DepartmentName { get => _departmentname; set => UpdateProperty(ref value, ref _departmentname); }
        public Department() { }

        public Department(DbDataReader reader) 
        {
            _departmentid = reader.GetInt64(0);
            _departmentname = reader.GetString(1);
        }

        public Department(long departmentid)
        {
            _departmentid = departmentid;
        }

        public override bool AllowUpdate()
        {
            return true;
        }

        public override ISQLModel Read(DbDataReader reader) => new Department(reader);

        public override string? ToString() => DepartmentName;

    }
}
