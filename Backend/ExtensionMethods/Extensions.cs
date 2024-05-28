using Backend.Database;

namespace Backend.ExtensionMethods
{
    public static class Extensions
    {
        public static Type GenericDatabase(this IAbstractDatabase db)
        {
            Type type = db.GetType();
            Type[] genericArguments = type.GetGenericArguments();
            return genericArguments[0];
        }

    }
}
