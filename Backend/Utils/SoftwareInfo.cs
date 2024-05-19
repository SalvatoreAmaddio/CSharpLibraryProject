using System.Reflection;
namespace Backend.Utils
{
    /// <summary>
    /// This class holds some information about the Developer and the Client.
    /// </summary>
    public class SoftwareInfo
    {
        public string DeveloperName { get; set; } = string.Empty;
        public Uri? DeveloperWebsite { get; set; }
        /// <summary>
        /// When the Software was Developed
        /// </summary>
        public string SoftwareYear { get; set; } = DateTime.Now.Year.ToString();
        
        /// <summary>
        /// This property is set by the <see cref="Assembly.GetName()"/> property.
        /// </summary>
        public string? SoftwareName { get; } = string.Empty;

        /// <summary>
        /// This property is set by the <see cref="Assembly.GetName()"/>.Version property.
        /// </summary>
        public string? SoftwareVersion { get; } = string.Empty;
        
        /// <summary>
        /// The name of the client this software was developed for.
        /// </summary>
        public string ClientName { get; set;} = string.Empty;
        
        /// <summary>
        /// This property refers to the <see cref="Assembly.GetEntryAssembly()"/>
        /// </summary>
        public Assembly? AppAssembly { get; }

        public SoftwareInfo() 
        {
            AppAssembly = Assembly.GetEntryAssembly();
            SoftwareName = AppAssembly?.GetName()?.Name;
            SoftwareVersion = $"v. {AppAssembly?.GetName()?.Version?.ToString()}";
        }

        public SoftwareInfo(string developerName, string developerWebsite, string client, string year) : this()
        {
            DeveloperName = developerName;
            DeveloperWebsite = new Uri($"https://{developerWebsite}");
            ClientName = client;
            SoftwareYear = year;
        }
    }
}