using Backend.Controller;
using Backend.Model;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontEnd.Forms
{
    public class CommandPanel : Control
    {
        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand UpdateCMD 
        {
            get => (ICommand)GetValue(UpdateCMDProperty);
            set => SetValue(UpdateCMDProperty, value);
        }


        public static readonly DependencyProperty UpdateCMDProperty =
        DependencyProperty.Register(nameof(UpdateCMD), typeof(ICommand), typeof(CommandPanel), new PropertyMetadata());

        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand DeleteCMD
        {
            get => (ICommand)GetValue(DeleteCMDProperty);
            set => SetValue(DeleteCMDProperty, value);
        }


        public static readonly DependencyProperty DeleteCMDProperty =
        DependencyProperty.Register(nameof(DeleteCMD), typeof(ICommand), typeof(CommandPanel), new PropertyMetadata());

        public ISQLModel CommandParameter
        {
            get => (ISQLModel)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(ISQLModel), typeof(CommandPanel), new PropertyMetadata());

        static CommandPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandPanel), new FrameworkPropertyMetadata(typeof(CommandPanel)));
        }
    }
}
