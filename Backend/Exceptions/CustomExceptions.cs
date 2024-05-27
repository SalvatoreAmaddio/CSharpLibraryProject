﻿using System.Runtime.InteropServices;

namespace Backend.Exceptions
{
    public class WorksheetException(string message) : COMException(message) { }
    public class WorkbookException(string message) : COMException(message) { }
    public class MissingExcelException() : Exception("Excel is not installed.") { }

    public class NoModelException() : Exception("No Model is set") { }
    public class NoNavigatorException() : Exception("No Navigator is set") { }
    public class AssemblyCreateInstanceFailure(string text) : Exception(text) { }
    public class CurrentUserNotSetException() : Exception("CurrentUser.Is property was not set.") { }
    public class InvalidTargetsException(string target1, string target2) : Exception($"{target1} and {target2} arguments cannot be null or empty strings") { }
    public class InvalidReceiver() : Exception("No Receiver information was provided") { };
    public class InvalidSender() : Exception("No Sender information was provided") { };
    public class InvalidHost() : Exception("Host was not provided") { };
    public class CredentialFailure(string text) : Exception(text) { };

    /// <summary>
    /// The Exception that is thrown when the attempt to load a DLL has failed.
    /// </summary>
    /// <param name="dllName">The DLL to load.</param>
    public class DLLLoadFailure(string dllName) : Exception($"Failed to load {dllName}") { }
    
    /// <summary>
    /// The Exception that is thrown when the attempt to load a function from an Assembly has failed. This happens if the function is not part of the Assembly.
    /// </summary>
    /// <param name="functionName">name of the function to fetch.</param>
    /// <param name="dllName">the name of the Assembly where the function was supposed to be.</param>
    public class ExtractionFunctionFailure(string functionName, string dllName) : Exception($"Failed to load {functionName} from {dllName}") { }

}
