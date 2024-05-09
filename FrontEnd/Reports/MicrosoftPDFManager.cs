using System.Management;

namespace FrontEnd.Reports
{
    public enum PortAction
    { 
        ADD = 0,
        REMOVE = 1,
    }
    public static class MicrosoftPDFManager
    {
        //SET THIS TO FALSE IN THE APP MANIFEST. YOU CAN ADD THE MANIFEST BY CLICKING ON ADD NEW FILE
        //<requestedExecutionLevel  level="requireAdministrator" uiAccess="false" />
        public static string FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Invoice.pdf";
        private static readonly string originalPort = "PORTPROMPT:";
        private static readonly string printerName = "Microsoft Print To PDF";
        private static ManagementScope? scope;

        private static ConnectionOptions Options() => new()
        {
            Impersonation = ImpersonationLevel.Impersonate,
            Authentication = AuthenticationLevel.PacketPrivacy,
            EnablePrivileges = true
        };

        private static void Connect()
        {
            scope = new ManagementScope(ManagementPath.DefaultPath, Options());
            scope.Connect();
        }

        private static ManagementObjectCollection Collection()
        {
            SelectQuery oSelectQuery = new SelectQuery(@"SELECT * FROM Win32_Printer WHERE Name = '" + printerName.Replace("\\", "\\\\") + "'");
            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(scope, @oSelectQuery);
            return oObjectSearcher.Get();
        }

        public static void SetPort()
        {
            Connect();
            var collection = Collection();
            
            foreach (ManagementObject oItem in collection) 
            {
                oItem.Properties["PortName"].Value = FileName;
                oItem.Put();
            }
        }
    }
}