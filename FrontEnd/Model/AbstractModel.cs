﻿using Backend.Model;
using FrontEnd.Events;
using FrontEnd.Notifier;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FrontEnd.Model
{
    public interface IAbstractModel : ISQLModel, INotifier 
    {
        /// <summary>
        /// It gets and sets a value that indicates if any property, which uses <see cref="UpdateProperty{T}(ref T, ref T, string)"/>, of a object extending <see cref="AbstractNotifier"/> has changed.
        /// </summary>
        /// <value>True if a property has changed.</value>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Occurs when the <see cref="IsDirty"/> property gets from true to false.
        /// This event is set and triggered by the <see cref="Forms.SubForm"/> class.
        /// </summary>
        public event OnDirtyChangedEventHandler? OnDirtyChanged;

        public void Undo();
    }

    /// <summary>
    /// This class extends the <see cref="AbstractSQLModel"/> and adds extra functionalities for UI purposes
    /// </summary>
    public abstract class AbstractModel : AbstractSQLModel, IAbstractModel
    {
        public event OnDirtyChangedEventHandler? OnDirtyChanged;
        bool _isDirty = false;
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
                RaisePropertyChanged(nameof(IsDirty));
                if (!value)
                    OnDirtyChanged?.Invoke(this, new(this));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;

        public void RaisePropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "")
        {
            SimpleTableField? field = AllFields.Find(s => s.Name.Equals(propName));
            field.Value = _backProp;
            field.Changed = true;

            BeforeUpdateArgs args = new(value, _backProp, propName);
            BeforeUpdate?.Invoke(this, args);
            if (args.Cancel) return;
            _backProp = value;
            IsDirty = true;
            RaisePropertyChanged(propName);
            AfterUpdate?.Invoke(this, args);
        }

        public void Undo() 
        { 
            foreach (var field in AllFields.Where(s=>s.Changed)) 
            {
                field.Property.SetValue(this, field.Value);
            }

            IsDirty = false;
        }
        public override bool AllowUpdate()
        {
            bool result = base.AllowUpdate();

            if (!result)
                MessageBox.Show($"Please fill all mandatory fields:\n{GetEmptyMandatoryFields()}","Something is missing");

            return result;
        }
    }
}

