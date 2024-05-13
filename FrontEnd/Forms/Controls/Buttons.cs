using FrontEnd.Controller;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract class that defines a set of common properties and methods for custom buttons which are binds to the Commands defined in the <see cref="AbstractFormController{M}"/>.
    /// </summary>
    public abstract class AbstractButton : Button 
    {
        protected abstract string CommandName { get; }
        public AbstractButton() => DataContextChanged += OnDataContextChanged;

        #region IsWithinList
        /// <summary>
        /// Sets a Relative Source Binding between the button's DataContext and the <see cref="Lista"/>'s DataContext.
        /// </summary>
        public bool IsWithinList
        {
            private get => (bool)GetValue(IsWithinListProperty);
            set => SetValue(IsWithinListProperty, value);
        }

        public static readonly DependencyProperty IsWithinListProperty =
            DependencyProperty.Register(nameof(IsWithinList), typeof(bool), typeof(AbstractButton), new PropertyMetadata(false, OnIsWithinListPropertyChanged));

        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isWithinList = (bool)e.NewValue;
            if (isWithinList) 
                ((AbstractButton)d).SetBinding(DataContextProperty, new Binding(nameof(DataContext))
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            else BindingOperations.ClearBinding(d, DataContextProperty);
        }
        #endregion

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IAbstractFormController controller) return;

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
    /// Instantiate SaveButton and binds it to the UpdateCMD Command defined in the <see cref="AbstractFormController{M}"/>
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
    /// Instantiate OpenButton and binds it to the OpenCMD Command defined in the <see cref="AbstractFormListController{M}"/>
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
