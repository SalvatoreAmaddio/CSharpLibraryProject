namespace Backend.Exceptions
{
    public class NoModelException() : Exception("No Model is set") { }
    public class NoNavigatorException() : Exception("No Navigator is set") { }
    public class LoadedAssemblyFailure() : Exception("Failed to create a connection object from LoadedAssembly") { }
    public class CurrentUserNotSetException() : Exception("CurrentUser.Is property was not set.") { }
    public class InvalidTargetsException(string target1, string target2) : Exception($"{target1} and {target2} arguments cannot be null or empty strings") { }
    public class InvalidReceiver() : Exception("No Receiver information was provided") { };
    public class InvalidSender() : Exception("No Sender information was provided") { };
    public class InvalidHost() : Exception("Host was not provided") { };
    public class CredentialFailure(string text) : Exception(text) { };

}
