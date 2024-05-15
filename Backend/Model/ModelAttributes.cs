using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Mandatory : Attribute
    {        
        public Mandatory() { }
       
    }
}
