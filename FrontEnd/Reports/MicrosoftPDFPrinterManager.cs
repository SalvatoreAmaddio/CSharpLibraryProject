using System.Management;
using System.Runtime.InteropServices;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class interacts with PDFDriverHelper.dll which add and removes PDF Printer's ports.
    /// Then, the <see cref="ManagementScope"/> set the Port as a Default Port that the Printer will use while printing.
    /// <para/>
    /// <c>IMPORTANT:</c>
    /// <para/>
    /// SET THIS TO FALSE IN THE APP MANIFEST. YOU CAN ADD THE MANIFEST BY CLICKING ON ADD NEW FILE
    /// <c>&lt;requestedExecutionLevel level="requireAdministrator" uiAccess="false"/></c>
    /// </summary>
    //<requestedExecutionLevel level="requireAdministrator" uiAccess="false"/>
    public class MicrosoftPDFPrinterManager
    {
        internal enum PortAction
        {
            ADD = 0,
            REMOVE = 1,
        }

        public string NewPortName { get; set; } = string.Empty;
        public string FilePath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{NewPortName}.pdf";

        private readonly string originalPort = "PORTPROMPT:";
        private readonly string printerName = "Microsoft Print To PDF";
        private ManagementScope? managementScope;

        [DllImport("PrinterPortManager.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint CreateDeletePort(int action, string portName);

        /// <summary>
        /// Initiates the <see cref="ManagementScope"/>'s connection. 
        /// This method is called by <see cref="GetPrinter"/>.
        /// </summary>
        private void Connect()
        {
            managementScope = new ManagementScope(ManagementPath.DefaultPath, new()
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.PacketPrivacy,
                EnablePrivileges = true
            });

            managementScope.Connect();
        }

        /// <summary>
        /// Gets the PDF Printer.
        /// </summary>
        /// <returns>A ManagementObject</returns>
        private ManagementObject? GetPrinter()
        {
            Connect();
            SelectQuery oSelectQuery = new(@"SELECT * FROM Win32_Printer WHERE Name = '" + printerName.Replace("\\", "\\\\") + "'");
            ManagementObjectSearcher oObjectSearcher = new(managementScope, @oSelectQuery);
            return oObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault();
        }

        /// <summary>
        /// Sets the Default Port to PORTPROMPT: and deletes the newly created Port
        /// </summary>
        public void ResetPort()
        {
            SetDefaultPort(true);
            CreateDeletePort((int)PortAction.REMOVE, FilePath);
        }

        /// <summary>
        /// Creates a New Port, then the <see cref="ManagementObject"/> sets it as a Default Port.
        /// </summary>
        public void SetPort()
        {
            CreateDeletePort((int)PortAction.ADD,FilePath);
            SetDefaultPort();
        }


        /// <summary>
        /// Sets the default Printer's Port.
        /// </summary>
        /// <param name="useOriginal">true if the default port should be PORTPROMPT:</param>
        /// <exception cref="Exception">Throw an exception if the Printer was not found.</exception>
        private void SetDefaultPort(bool useOriginal = false)
        {
            ManagementObject? printer = GetPrinter() ?? throw new Exception("Printer not found!");
            printer.Properties["PortName"].Value = (useOriginal) ? originalPort : FilePath;
            printer.Put();
        }
    }
}