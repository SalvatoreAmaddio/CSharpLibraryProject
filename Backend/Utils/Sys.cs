using System.Reflection;
using System.Runtime.InteropServices;

namespace Backend.Utils
{
    public class Sys
    {
        /// <summary>
        /// Collection of Loaded Assemblies. See <see cref="LoadedAssembly"/>
        /// </summary>
        public static List<LoadedAssembly> LoadedDLL { get; } = [];

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

        /// <summary>
        /// It loads a EmbeddedResource dll 
        /// </summary>
        /// <param name="dllName">The name of the dll</param>
        /// <exception cref="Exception">Resource not found Exception</exception>
        public static void LoadEmbeddedDll(string dllName)
        {
            string architecture = IntPtr.Size == 8 ? "bit64" : "x86";
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string resourceName = $"Backend.Database.{architecture}.{dllName}.dll";

            using (Stream? stream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new Exception($"Resource {resourceName} not found.");

                string tempFile = Path.Combine(Path.GetTempPath(), dllName);
                using (FileStream fs = new(tempFile, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fs);
                }

                if (!NativeLibrary.TryLoad(tempFile, out IntPtr handle)) throw new Exception($"Failed to load DLL: {tempFile}");

                LoadedAssembly assembly = new(tempFile, dllName, IntPtr.Size == 8 ? "x64" : "x86");
                assembly.Load();
                LoadedDLL.Add(assembly);
            }
        }

    }
    
    /// <summary>
    /// Represent an object holding a reference to an external Assembly.
    /// </summary>
    /// <param name="path">The path were the assembly is located.</param>
    public class LoadedAssembly(string path, string name, string architecture)
    {
        /// <summary>
        /// Gets the Loaded Assembly's Architecture.
        /// </summary>
        public string Architecture { get; } = architecture;

        /// <summary>
        /// Gets the name of the DLL.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the path of the DLL.
        /// </summary>
        public string Path { get; } = path;

        /// <summary>
        /// Gets the actual loaded Assembly
        /// </summary>
        /// <returns>An Assembly</returns>
        public Assembly? Assembly { get; private set; } 

        /// <summary>
        /// Load the assembly.
        /// </summary>
        public void Load() => Assembly = Assembly.LoadFile(Path);

        public override string? ToString() => $"{Name}.dll - Architecture: {Architecture}";

    }
}
