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
    /// This class interacts with PDFDriverHelper.exe which add and removes PDF Printer's ports.
    /// Then, the <see cref="ManagementScope"/> set the Port as a Default Port that the Printer will use while printing.
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

        private static ConnectionOptions Options() => new()
        {
            Impersonation = ImpersonationLevel.Impersonate,
            Authentication = AuthenticationLevel.PacketPrivacy,
            EnablePrivileges = true
        };

        /// <summary>
        /// Connects to the <see cref="ManagementScope"/>. 
        /// This method is called by <see cref="SetDefaultPort(bool)"/> before it attempts to set the Printer's Port.
        /// </summary>
        private void Connect()
        {
            scope = new ManagementScope(ManagementPath.DefaultPath, Options());
            scope.Connect();
        }

        /// <summary>
        /// Gets the PDF Printer.
        /// </summary>
        /// <returns>A ManagementObject</returns>
        private ManagementObject? GetPrinter()
        {
            SelectQuery oSelectQuery = new(@"SELECT * FROM Win32_Printer WHERE Name = '" + printerName.Replace("\\", "\\\\") + "'");
            ManagementObjectSearcher oObjectSearcher = new(scope, @oSelectQuery);
            return oObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault();
        }

        /// <summary>
        /// Executes PrinterPortManager.exe.
        /// </summary>
        /// <param name="action">A <see cref="PortAction"/> enum</param>
        /// <returns>A Task which returns an integer telling the result of PrinterPortManager.exe. If -1, something went wrong.</returns>
        private async Task<int> RunPortManagerAsync(PortAction action)
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

        /// <summary>
        /// Sets the Default Port to PORTPROMPT: and executes PrinterPortManager.exe which deletes the Newly created Port
        /// </summary>
        /// <returns>A Task which returns an integer telling the result of PrinterPortManager.exe. If -1, something went wrong.</returns>
        public async Task<int> ResetPort()
        {
            SetDefaultPort(true);
            return await RunPortManagerAsync(PortAction.REMOVE);
        }

        /// <summary>
        /// Executes PrinterPortManager.exe which create a New Port, then the <see cref="ManagementObject"/> sets it as a Default Port.
        /// </summary>
        /// <returns>A Task which returns an integer telling the result of PrinterPortManager.exe. If -1, something went wrong.</returns>
        public async Task<int> SetPort()
        {
            int result = await RunPortManagerAsync(PortAction.ADD);
            SetDefaultPort();
            return result;
        }


        /// <summary>
        /// Sets the default Printer's Port.
        /// </summary>
        /// <param name="useOriginal">true if the default port should be PORTPROMPT:</param>
        /// <exception cref="Exception">Throw an exception if the Printer was not found.</exception>
        private void SetDefaultPort(bool useOriginal = false)
        {
            Connect();
            ManagementObject? printer = GetPrinter() ?? throw new Exception("Printer not found!");
            printer.Properties["PortName"].Value = (useOriginal) ? originalPort : FilePath;
            printer.Put();
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