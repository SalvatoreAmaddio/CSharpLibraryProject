using System.Management;
using System.Runtime.InteropServices;

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
        public static string FileName { get; set; } = string.Empty;
        private static string FilePath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{FileName}.pdf";
        private static readonly string originalPort = "PORTPROMPT:";
        private static readonly string printerName = "Microsoft Print To PDF";
        private static ManagementScope? scope;

        [DllImport("PrinterPortManager.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint CreateDeletePort(PortAction action, string portName, IntPtr printerObject);

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
            ManagementObjectSearcher oObjectSearcher = new(scope, @oSelectQuery);
            return oObjectSearcher.Get();
        }

        public static void SetPort()
        {
            IntPtr printerObject = IntPtr.Zero; // Handle
            var result = CreateDeletePort(PortAction.ADD, FilePath, printerObject);
            if (result != 0) throw new Exception();
            
            Connect();
            var collection = Collection();
            
            foreach (ManagementObject oItem in collection) 
            {
                oItem.Properties["PortName"].Value = FilePath;
                oItem.Put();
            }
        }
    }
}