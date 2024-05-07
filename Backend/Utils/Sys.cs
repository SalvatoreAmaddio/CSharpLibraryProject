using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils
{
    public class Sys
    {

        /// <summary>
        /// Check if an object is a number.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if the object is a number</returns>
        public static bool IsNumber(object? obj)
        {
            if (obj == null) return false;

            Type objType = obj.GetType();
            return objType == typeof(int) || objType == typeof(double) ||
                   objType == typeof(decimal) || objType == typeof(float) ||
                   objType == typeof(long) || objType == typeof(short) ||
                   objType == typeof(ulong) || objType == typeof(ushort) ||
                   objType == typeof(sbyte) || objType == typeof(byte);
        }

    }
}
