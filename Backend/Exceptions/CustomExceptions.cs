﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    public class NoModelException : Exception
    { 
        public NoModelException() : base("No Model is set") { }
    }
}
