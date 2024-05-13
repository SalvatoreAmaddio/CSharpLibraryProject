using System.Reflection;
using System.Runtime.InteropServices;

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

        public static void LoadEmbeddedDll(string dllName)
        {
            string architecture = IntPtr.Size == 8 ? "bit64" : "x86";
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Backend.Database.{architecture}.{dllName}";

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Resource {resourceName} not found.");
                }

                string tempFile = Path.Combine(Path.GetTempPath(), dllName);
                using (FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fs);
                }

                LoadDll(tempFile);
            }
        }

        private static void LoadDll(string dllPath)
        {
            IntPtr handle = LoadLibrary(dllPath);
            if (handle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Exception(error.ToString());
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);

    }
}
