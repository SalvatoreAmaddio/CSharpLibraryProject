using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows;

namespace FrontEnd.Reports
{
    public enum PortAction
    { 
        ADD = 0,
        REMOVE = 1,
    }

    /// <summary>
    /// This class interacts with PDFDriverHelper.exe to add and removes PDF Printer's ports.
    /// <para/>
    /// <c>IMPORTANT:</c>
    /// <para/>
    /// SET THIS TO FALSE IN THE APP MANIFEST. YOU CAN ADD THE MANIFEST BY CLICKING ON ADD NEW FILE
    /// <c>&lt;requestedExecutionLevel  level="requireAdministrator" uiAccess="false"/></c>
    /// </summary>
    //<requestedExecutionLevel  level="requireAdministrator" uiAccess="false" />
    public class MicrosoftPDFPrinterManager
    {
        public string NewPortName { get; } = string.Empty;
        private string FilePath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{NewPortName}.pdf";

        private readonly string originalPort = "PORTPROMPT:";
        private readonly string printerName = "Microsoft Print To PDF";
        private readonly string c_App = "\\PDFDriverHelper.exe";
        private ManagementScope? scope;

        [DllImport("PrinterPortManager.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint CreateDeletePort(int action, string portName);

        private ConnectionOptions Options() => new()
        {
            Impersonation = ImpersonationLevel.Impersonate,
            Authentication = AuthenticationLevel.PacketPrivacy,
            EnablePrivileges = true
        };

        private void Connect()
        {
            scope = new ManagementScope(ManagementPath.DefaultPath, Options());
            scope.Connect();
        }

        private ManagementObjectCollection Collection()
        {
            SelectQuery oSelectQuery = new SelectQuery(@"SELECT * FROM Win32_Printer WHERE Name = '" + printerName.Replace("\\", "\\\\") + "'");
            ManagementObjectSearcher oObjectSearcher = new(scope, @oSelectQuery);
            return oObjectSearcher.Get();
        }

        public async Task<int> RunPortManagerAsync(PortAction action)
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
            Process process = new()
            {
                StartInfo = StartInfo
            };
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode;
        }

        public async Task<int> ResetPort()
        {
            SetDefaultPort(true);
            return await RunPortManagerAsync(PortAction.REMOVE);
        }

        public async Task<int> SetPort()
        {
            int result = await RunPortManagerAsync(PortAction.ADD);
            SetDefaultPort();
            return result;
        }

        private void SetDefaultPort(bool useOriginal = false)
        {
            Connect();
            var collection = Collection();

            foreach (ManagementObject oItem in collection)
            {
                oItem.Properties["PortName"].Value = (useOriginal) ? originalPort : FilePath;
                oItem.Put();
            }
        }
        
        public void OpenFile()
        {
            try
            {
                Process.Start(new ProcessStartInfo(FilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open the PDF file. Error: {ex.Message}");
            }
        }
    }
}