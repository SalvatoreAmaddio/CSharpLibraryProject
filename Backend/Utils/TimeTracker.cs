using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils
{
    public class TimeTracker : Stopwatch
    {
        public override string ToString()
        {
            TimeSpan elapsedTime = this.Elapsed;
            return $"Elapsed Time: {elapsedTime.Hours} hours, {elapsedTime.Minutes} minutes, {elapsedTime.Seconds} seconds";
        }
    }
}
