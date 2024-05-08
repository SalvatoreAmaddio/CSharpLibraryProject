using FrontEnd.Controller;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    public abstract class AbstractButton : Button 
    {
        protected abstract string CommandName { get; }
        public AbstractButton() 
        {
            DataContextChanged += OnDataContextChanged;

        }

       private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IAbstractController controller = (IAbstractController)e.NewValue;
            Binding CommandParameterBinding = new()
            {
                Source = controller,
                Path = new("CurrentRecord")
            };

            SetBinding(CommandParameterProperty, CommandParameterBinding);

            Binding CommandBinding = new()
            {
                Source = controller,
                Path = new(CommandName)
            };

            SetBinding(CommandProperty, CommandBinding);

        }
    }
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
