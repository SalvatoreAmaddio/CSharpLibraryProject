﻿using System.Runtime.InteropServices;
using System.Text;

namespace Backend.Utils
{
    /// <summary>
    /// This class stores sensitive information in the Windows Credential Manager System. The information is stored in the local computer
    /// </summary>
    public static class CredentialManager
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct CREDENTIAL
        {
            public uint Flags;
            public uint Type;
            public string TargetName;
            public string Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public string TargetAlias;
            public string UserName;
        }

        private const uint CRED_TYPE_GENERIC = 1;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] uint flags);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CredRead(string target, uint type, uint flags, out IntPtr credential);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CredDelete(string target, uint type, uint flags);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern void CredFree([In] IntPtr buffer);

        /// <summary>
        /// Stores credential's information in the Windows Credential Manager System.
        /// </summary>
        /// <param name="target">a string that works as the credential unique identifier</param>
        /// <param name="username">a string that works as the name of the information to store</param>
        /// <param name="password">a string that rapresent the actual sensitive information to store</param>
        /// <returns>true if the credential was successfully stored</returns>
        public static bool Store(Credential cred)
        {
            var byteArray = Encoding.Unicode.GetBytes(cred.Password);

            var credential = new CREDENTIAL
            {
                TargetName = cred.Target,
                UserName = cred.Username,
                CredentialBlob = Marshal.StringToCoTaskMemUni(cred.Password),
                CredentialBlobSize = (uint)byteArray.Length,
                Type = CRED_TYPE_GENERIC,
                Persist = 2  // CRED_PERSIST_LOCAL_MACHINE
            };

            bool result = CredWrite(ref credential, 0);

            Marshal.FreeCoTaskMem(credential.CredentialBlob);
            return result;
        }

        /// <summary>
        /// Retrieve the credential stored in the Windows Credential Manager System.
        /// </summary>
        /// <param name="target">a string that works as the credential unique identifier</param>
        /// <returns>the information value as a string.</returns>
        public static string? Get(string target)
        {
            bool result = CredRead(target, CRED_TYPE_GENERIC, 0, out IntPtr credentialPtr);
            if (result)
            {
                    object? pointer = Marshal.PtrToStructure(credentialPtr, typeof(CREDENTIAL));
                    if (pointer == null) throw new Exception();
                    CREDENTIAL credential = (CREDENTIAL)pointer;
                    string password = Marshal.PtrToStringUni(credential.CredentialBlob, (int)(credential.CredentialBlobSize / 2));
                    CredFree(credentialPtr);
                    return password;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the credential stored in the Windows Credential Manager System.
        /// </summary>
        /// <param name="target">a string that works as the credential unique identifier</param>
        /// <returns>true if the credential was successfully removed</returns>
        public static bool Delete(string target) => CredDelete(target, CRED_TYPE_GENERIC, 0);
    }

    /// <summary>
    /// Instantiates an object holding Credential's information.
    /// </summary>
    /// <param name="target">The Credential Unique Identifier</param>
    /// <param name="username">The Name of the information to store.</param>
    /// <param name="password">The actual sensitive information to store.</param>
    public class Credential(string target, string username, string password) 
    {
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public string Target { get; private set; } = target;

        /// <summary>
        /// Name of the information to store.
        /// </summary>
        public string Username { get; private set; } = username;

        /// <summary>
        /// The actual sensitive information to store.
        /// </summary>
        public string Password { get; private set; } = password;

        public override bool Equals(object? obj)
        {
            return obj is Credential credential &&
                   Target == credential.Target;
        }

        public override int GetHashCode() => HashCode.Combine(Target);
    }
}
