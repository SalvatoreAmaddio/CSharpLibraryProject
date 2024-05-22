using System.Reflection;
using System.Runtime.InteropServices;

namespace Backend.Utils
{
    /// <summary>
    /// Represent an object holding a reference to an external Assembly.
    /// </summary>
    /// <param name="path">The path were the assembly is located.</param>
    public class LoadedAssembly(string path, string name, string architecture)
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

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
        public void Load() 
        {
            try 
            {
                Assembly = Assembly.LoadFile(Path);
            }
            catch 
            { 
                
            }
        } 

        /// <summary>
        /// Returns a function from the Assembly as the type of delegate specified by the Generic D.
        /// </summary>
        /// <typeparam name="D">The type of Delegate</typeparam>
        /// <param name="functionName">The name of the function to load</param>
        /// <returns>A delegate</returns>
        /// <exception cref="Exception"></exception>
        public D LoadFunction<D>(string functionName) 
        {
            IntPtr hModule = LoadLibrary(Path);
            if (hModule == IntPtr.Zero)
                throw new Exception("Failed to load dll");
            IntPtr pFunc = GetProcAddress(hModule, functionName);
            if (pFunc == IntPtr.Zero)
            {
                FreeLibrary(hModule);
                throw new Exception($"Failed to load {functionName} from {Name}");
            }
            D? delegateFunction = Marshal.GetDelegateForFunctionPointer<D>(pFunc);
            FreeLibrary(hModule);
            return delegateFunction;
        }

        public override string? ToString() => $"{Name}.dll - Architecture: {Architecture}";

    }
}
