using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;
using System.Windows;

namespace MyApplication.Model
{
    [Table(nameof(Employee))]
    public class Employee : AbstractModel
    {
        long _employeeid;
        string _firstName=string.Empty;
        string _lastName = string.Empty;
        Gender? _gender;
        Department? _department;
        DateTime? _dob;
        JobTitle? _jobTitle;
        string _email = string.Empty;

        [PK]
        public long EmployeeID { get => _employeeid; set => UpdateProperty(ref value, ref _employeeid); }

        [Mandatory]
        [Field]
        public string FirstName { get => _firstName; set => UpdateProperty(ref value, ref _firstName); }

        [Mandatory]
        [Field]
        public string LastName { get => _lastName; set => UpdateProperty(ref value, ref _lastName); }

        [Mandatory]
        [Field]
        public DateTime? DOB { get => _dob; set => UpdateProperty(ref value, ref _dob); }

        [Mandatory]
        [FK]
        public Gender? Gender { get => _gender; set => UpdateProperty(ref value, ref _gender); }

        [Mandatory]
        [FK]
        public Department? Department { get => _department; set => UpdateProperty(ref value, ref _department); }

        [Mandatory]
        [FK]
        public JobTitle? JobTitle { get => _jobTitle; set => UpdateProperty(ref value, ref _jobTitle); }

        [Mandatory]
        [Field]
        public string Email { get => _email; set => UpdateProperty(ref value, ref _email); }

        public Employee(DbDataReader reader)
        {
            _employeeid = reader.GetInt64(0);
            _firstName = reader.GetString(1);
            _lastName = reader.GetString(2);
            _dob = reader.GetDateTime(3);
            _gender = new(reader.GetInt64(4));
            _department = new(reader.GetInt64(5));
            _jobTitle = new(reader.GetInt64(6));
            _email = reader.GetString(7);
        }

        public Employee(long id) => _employeeid = id;

        public Employee() { }

        public override ISQLModel Read(DbDataReader reader) => new Employee(reader);

        public override string? ToString() => FirstName;
    }
}