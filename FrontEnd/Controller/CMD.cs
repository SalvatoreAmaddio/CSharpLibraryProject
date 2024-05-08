using System.Windows.Input;
using Backend.Model;

namespace FrontEnd.Controller
{
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

    public class CMD<M>(Action<M?> execute) : ICommand where M : ISQLModel, new()
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
