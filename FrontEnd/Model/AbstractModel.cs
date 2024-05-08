﻿using Backend.Model;
using FrontEnd.Events;
using FrontEnd.Notifier;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEnd.Model
{
    public abstract class AbstractModel : AbstractSQLModel, INotifier
    {
        bool _isDirty = false;
        public bool IsDirty
        {
            get => _isDirty;
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
