using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using CredentialManagement;
using Org.BouncyCastle.Asn1.X509;

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
        public static Credential GetCredentials(string target) 
        {
            Credential cm = new () { Target = target };
            cm.Load();
            return cm;
        }

        /// <summary>
        /// Removes sensitive information in the local computer's Windows Credential Manager System.
        /// </summary>
        /// <param name="target">A name that works as the unique identifier of the credential</param>
        /// <returns>true if the credential has been successfully removed</returns>
        public static bool RemoveCredentials(string target)
        {
            using (var cred = new Credential() { Target = target })
            {
                return cred.Delete();
            }
        }

        /// <summary>
        /// Safely stores sensitive information in the local computer's Windows Credential Manager System.
        /// </summary>
        /// <param name="credentiaID">A name that works as unique identifier</param>
        /// <param name="username">A string representing the name of the information to store</param>
        /// <param name="password">The actual value that must be safely stored</param>
        /// <returns>true if the credential has been successfully stored</returns>
        public static bool StoreCredentials(string credentiaID, string username, string password) 
        {
            using (var cred = new Credential())
            {
                cred.Target = credentiaID;
                cred.Username = username;
                cred.Password = password;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                return cred.Save();
            }
        }

        //public static void StoreCredentials(string target, string username, string password)
        //{
        //    try
        //    {
        //        string cmd = $"cmdkey /add:{target} /user:{username} /pass:{password}";
        //        ProcessStartInfo psi = new ("cmd.exe", "/c " + cmd)
        //        {
        //            RedirectStandardOutput = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //        using (Process process = Process.Start(psi))
        //        {
        //            process?.WaitForExit();
        //            if (process.ExitCode == 0)
        //                Console.WriteLine("Credentials stored successfully.");
        //            else
        //                Console.WriteLine("Failed to store credentials.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception: {ex.Message}");
        //    }
        //}
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
