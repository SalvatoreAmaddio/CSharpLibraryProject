
namespace FrontEnd.Events
{
    public delegate void AfterUpdateEventHandler(object? sender, AfterUpdateArgs e);
    public delegate void BeforeUpdateEventHandler(object? sender, BeforeUpdateArgs e);

    public abstract class UpdateArgs(string propertyName) : EventArgs
    {
        public readonly string propertyName = propertyName;

        /// <summary>
        /// Check the if current property name is equal to the given value. 
        /// </summary>
        /// <param name="value">the name of a Property</param>
        /// <returns>bool if the current property name is equal to the given value</returns>
        public bool Is(string value) => propertyName.Equals(value);

    }

    public class AfterUpdateArgs(object? value, object? backProperty, string propertyName) : UpdateArgs(propertyName)
    {
        public object? value = value;
        public readonly object? backProperty = backProperty;
    }

    public class BeforeUpdateArgs(object? value, object? backProperty, string propertyName) : AfterUpdateArgs(value, backProperty, propertyName)
    {
        /// <summary>
        /// This property is used to notify if an update can be processed or not.
        /// </summary>
        /// <value>True is the property can be updated,</value>
        public bool Cancel { get; set; } = false;
    }
}
