
namespace Backend.Exceptions
{
    public class NoModelException() : Exception("No Model is set") { }
    public class NoNavigatorException() : Exception("No Navigator is set") { }

    public class CurrentUserNotSetException() : Exception("CurrentUser.Is property was not set.") { }
}
