using System.Windows.Input;

namespace FrontEnd.Controller
{
    public class CMD<M>(Action<M?> execute) : ICommand where M : Backend.Model.ISQLModel, new()
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
