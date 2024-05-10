using System.Diagnostics;
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

        private static Process? process;
        private static readonly string originalPort = "PORTPROMPT:";
        private static readonly string printerName = "Microsoft Print To PDF";
        private static readonly string c_App = "\\PDFDriverHelper.exe";
        private static ManagementScope? scope;

        [DllImport("PrinterPortManager.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint CreateDeletePort(int action, string portName);

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

        public static Task<bool> RunPortManagerAsync(PortAction action)
        {
            ProcessStartInfo StartInfo = new()
            {
                FileName = "C:\\Users\\salva\\source\\repos\\CSharpLibraryProject\\MyApplication\\bin\\Debug\\net8.0-windows\\PrinterPortManager.exe",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            StartInfo.ArgumentList.Add(FilePath);
          
            StartInfo.ArgumentList.Add(((int)action).ToString());
            process = new()
            {
                StartInfo = StartInfo
            };
            process.Start();
            return Task.FromResult(process.HasExited);
        }

        public static void SetDefaultPort()
        {
            Connect();
            var collection = Collection();

            foreach (ManagementObject oItem in collection)
            {
                oItem.Properties["PortName"].Value = FilePath;
                oItem.Put();
            }
            process?.Kill();
        }
    }
}