using FrontEnd.Model;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class implements <see cref="ICommand"/>
    /// </summary>
    /// <param name="execute">An Action</param>
    public class CMD(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action _execute = execute;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter) 
        {
            _execute();
        }
    }

    /// <summary>
    /// This class implements <see cref="ICommand"/> and deal with click events that take a <see cref="AbstractModel"/> object as argument
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    /// <param name="execute">An <see cref="Action{T}"/></param>
    public class CMD<M>(Action<M?> execute) : ICommand where M : AbstractModel, new()
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action<M?> _execute = execute;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _execute((M?)parameter);
        }
    }
}
