using Backend.Database;
using Backend.Model;

namespace Backend.Utils
{
    /// <summary>
    /// This class is meant to hold a static referecen to the current user. It wraps Methods and Properties of the <see cref="IUser"/> interface to be used globally.
    /// </summary>
    public class CurrentUser
    {
        /// <summary>
        /// Sets the Current User.
        /// </summary>
        public static IUser? Is { private get; set; }

        /// <summary>
        /// Gets the current UserID.
        /// </summary>
        public static long UserID
        {
            get
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.UserID;
            }
            private set
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                Is.UserID = value;
            }
        }

        /// <summary>
        /// Gets ans Sets the current UserName.
        /// </summary>
        public static string UserName
        { 
            get 
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.UserName;
            }
            set 
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                Is.UserName = value;
            }
        }

        /// <summary>
        /// Gets ans Sets the current User's Password.
        /// </summary>
        public static string Password
        {
            get
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.Password;
            }
            set
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                Is.Password = value;
            }
        }

        /// <summary>
        /// Gets and Sets a flag to save User's access as a <see cref="Credential"/> object in the Local Computer.
        /// </summary>
        public static bool RememberMe
        {
            get
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.RememberMe;
            }
            set
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                Is.RememberMe = value;
            }
        }

        /// <summary>
        /// Gets the number of Attempts left for the Current User.
        /// </summary>
        public static int Attempts
        {
            get
            {
                if (Is == null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.Attempts;
            }
        }

        /// <summary>
        /// Gets the Target for the CurrentUser to deal with <see cref="CredentialManager"/> operations.
        /// </summary>
        public static string Target 
        { 
            get 
            {
                if (Is== null)
                    throw new ArgumentNullException(nameof(Is));
                return Is.Target;
            }
        }

        /// <summary>
        /// Retrieves the password from the Database based on the username provided. 
        /// This password can be checked against another password provided by 
        /// the user for login's purposes.<para/>
        /// <c>IMPORTANT:</c>
        /// This method does not change the <see cref="Password"/> property.
        /// </summary>
        /// <param name="decrypt">true if the Password should be decrypted</param>
        /// <returns>A password if the user was found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string? FetchUserPassword(bool decrypt = false) 
        {
            if (Is == null) throw new ArgumentNullException(nameof(Is));
            List<QueryParameter> para = [new(nameof(UserName), Is.UserName)];
            IUser? user = DatabaseManager.Do.Find("User")?.Retrieve(null, para).Cast<IUser>().FirstOrDefault();
            if (user == null) return null;
            UserID = user.UserID;
            return (decrypt) ? new Encrypter(user.Password).Decrypt() : user.Password;
        }

        /// <summary>
        /// It changes the password for the Current User. The new password will be encrypted.
        /// </summary>
        /// <param name="pwd">The new password</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ChangePassword(string pwd)
        {
            if (Is == null) throw new ArgumentNullException(nameof(Is));
            Encrypter encrypter = new(pwd);
            Password = encrypter.Encrypt();
            encrypter.ReplaceStoredKey();
            encrypter.ReplaceStoredIV();
            List<QueryParameter> para = [new(nameof(Password), Is.Password), new(nameof(Is.UserID), Is.UserID)];
            DatabaseManager.Do.Find("User")?.Crud(CRUD.UPDATE, $"UPDATE User SET Password=@Password WHERE UserID=@UserID", para);
        }

        /// <summary>
        /// Attempts to login.
        /// </summary>
        /// <param name="pwd">The password to check</param>
        /// <returns>true if the login was successful</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Login(string? pwd) 
        {
            if (Is == null)
                throw new ArgumentNullException(nameof(Is));
            return Is.Login(pwd);
        }

        /// <summary>
        /// Resets the <see cref="Attempts"/> property to its initial value.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ResetAttempts()
        {
            if (Is == null)
                throw new ArgumentNullException(nameof(Is));
            Is.ResetAttempts();
        }

        /// <summary>
        /// Attempts to get and read the <see cref="Credential"/> object.
        /// If found, it sets the <see cref="UserName"/> and <see cref="Password"/> properties.
        /// </summary>
        /// <returns>true if the credential was found and read.</returns>
        public static bool ReadCredential()
        {
            Credential? credential = CredentialManager.Get(Target);
            if (credential == null) return false;
            UserName = credential.Username;
            Password = credential.Password;
            return true;
        }

        /// <summary>
        /// Logs out and remove any User's <see cref="Credential"/> object stored in the Local Computer.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Logout()
        {
            if (Is == null) throw new ArgumentNullException(nameof(Is));
            Is.Logout();
        }
    }
}
