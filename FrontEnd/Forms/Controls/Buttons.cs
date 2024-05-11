using FrontEnd.Controller;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract class that defines a set of common properties and methods for custom buttons which are binds to the Commands defined in the <see cref="AbstractController{M}"/>.
    /// </summary>
    public abstract class AbstractButton : Button 
    {
        protected abstract string CommandName { get; }
       // public AbstractButton() => DataContextChanged += OnDataContextChanged;

       private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IAbstractController controller = (IAbstractController)e.NewValue;
            Binding CommandParameterBinding = new("CurrentRecord")
            {
                Source = controller,
            };

            SetBinding(CommandParameterProperty, CommandParameterBinding);

            Binding CommandBinding = new(CommandName)
            {
                Source = controller,
            };

            SetBinding(CommandProperty, CommandBinding);

        }
    }

    /// <summary>
    /// Instantiate SaveButton and binds it to the UpdateCMD Command defined in the <see cref="AbstractController{M}"/>
    /// </summary>
    public class SaveButton : AbstractButton
    {
        protected override string CommandName => "UpdateCMD";
        public SaveButton() 
        {
            ToolTip = "Save";
            Content = new Image()
            {
                Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/save.png")
            };
        }

    }

    /// <summary>
    /// Instantiate DeleteButton and binds it to the DeleteCMD Command defined in the <see cref="AbstractContentControl"/>
    /// </summary>
    public class DeleteButton : AbstractButton
    {
        protected override string CommandName => "DeleteCMD";
        public DeleteButton()
        {
            ToolTip = "Delete";
            Content = new Image()
            {
                Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/delete.png")
            };
        }
    }

    /// <summary>
    /// Instantiate OpenButton and binds it to the OpenCMD Command defined in the <see cref="AbstractListController{M}"/>
    /// </summary>
    public class OpenButton : AbstractButton
    {
        protected override string CommandName => "OpenCMD";

        public OpenButton()
        {
            ToolTip = "Open";
            Content = new Image()
            {
                Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/folder.png")
            };
        }
    }
}
