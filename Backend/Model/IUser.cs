
namespace Backend.Model
{
    public interface IUser
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public int Attempts { get; }
        public string Target { get; set; }

        /// <summary>
        /// It attempts to login.
        /// </summary>
        /// <param name="pwd">The password to be checked against</param>
        /// <returns>true if the login attempt was successful</returns>
        /// <exception cref="Exception"></exception>
        public bool Login(string pwd);

        public void DeleteCredential();

        public void SaveCredentials();

    }
}
