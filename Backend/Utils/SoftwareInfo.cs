using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils
{
    public class SoftwareInfo
    {
        public string DeveloperName { get; set; } = string.Empty;
        public Uri? DeveloperWebsite { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SoftwareYear { get; set; } = DateTime.Now.Year.ToString();
        public string? SoftwareName { get; } = string.Empty;
        public string? SoftwareVersion { get; } = string.Empty;
        public string ClientName { get; set;} = string.Empty;
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
