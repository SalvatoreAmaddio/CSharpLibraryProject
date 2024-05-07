using Backend.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Model
{
    /// <summary>
    /// This class is the concrete implementations of <see cref="INotifier"/>
    /// </summary>
    abstract public class AbstractNotifier : INotifier
    {
        bool _isDirty = false;
        public bool IsDirty { get => _isDirty; 
            set 
            {
                _isDirty = value;
                RaisePropertyChanged(nameof(IsDirty));
            } 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;

        public void RaisePropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "")
        {
            BeforeUpdateArgs args = new(value, _backProp, propName);
            BeforeUpdate?.Invoke(this, args);
            if (args.Cancel) return;
            _backProp = value;
            IsDirty = true;
            RaisePropertyChanged(propName);
            AfterUpdate?.Invoke(this, args);
        }
    }

}
