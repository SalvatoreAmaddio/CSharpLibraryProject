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
        public string DeveloperName { get; set; } = "Salvatore Amaddio";
        public Uri DeveloperWebsite { get; set; } = new("https://www.salvatoreamaddio.co.uk");
        public string Description { get; set; } = string.Empty;
        public string SoftwareYear { get; set; } = "2023 - 2024";
        public string? SoftwareName { get; set; } = string.Empty;
        public string? SoftwareVersion { get; set; } = string.Empty;
        public Assembly? AppAssembly { get; }
        public SoftwareInfo() 
        {
            AppAssembly = Assembly.GetEntryAssembly();
            SoftwareName = AppAssembly?.GetName()?.Name;
            SoftwareVersion = $"v. {AppAssembly?.GetName()?.Version?.ToString()}";
        }
    }
}
