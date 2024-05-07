using Backend.Database;
using Backend.Model;
using Backend.Utils;
using System.Data;
using System.Data.Common;

namespace MyApplication.Model
{
    [Table(nameof(Person))]
    public class Person : AbstractModel
    {
        long _personid;
        string _firstName=string.Empty;
        string _lastName = string.Empty;
        Gender? _gender;
        Department? _department;
        DateTime? _dob;
        JobTitle? _jobTitle;

        [PK]
        public long PersonID { get => _personid; set => UpdateProperty(ref value, ref _personid); }
        [Field]
        public string FirstName { get => _firstName; set => UpdateProperty(ref value, ref _firstName); }
        [Field]
        public string LastName { get => _lastName; set => UpdateProperty(ref value, ref _lastName); }

        [Field]
        public DateTime? DOB { get => _dob; set => UpdateProperty(ref value, ref _dob); }

        [FK]
        public Gender? Gender { get => _gender; set => UpdateProperty(ref value, ref _gender); }

        [FK]
        public Department? Department { get => _department; set => UpdateProperty(ref value, ref _department); }

        [FK]
        public JobTitle? JobTitle { get => _jobTitle; set => UpdateProperty(ref value, ref _jobTitle); }

        public Person(DbDataReader reader)
        {
            _personid = reader.GetInt64(0);
            _firstName = reader.GetString(1);
            _lastName = reader.GetString(2);
            _dob = reader.GetDateTime(3);
            _gender = new(reader.GetInt64(4));
            _department = new(reader.GetInt64(5));
            _jobTitle = new(reader.GetInt64(6));
        }

        public Person() 
        {
        }

        public override bool AllowUpdate()
        {
            return true;
        }

        public override ISQLModel Read(DbDataReader reader) => new Person(reader);

        public override string? ToString() => FirstName;
    }
}