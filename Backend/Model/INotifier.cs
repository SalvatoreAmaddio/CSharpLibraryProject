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
    /// This interface extends <see cref="INotifyPropertyChanged"/> and adds extra functionalities.
    /// </summary>
    public interface INotifier : INotifyPropertyChanged
    {
        /// <summary>
        /// It gets and sets a value that indicates if any property, which uses <see cref="UpdateProperty{T}(ref T, ref T, string)"/>, of a object extending <see cref="AbstractNotifier"/> has changed.
        /// </summary>
        /// <value>True if a property has changed.</value>
        public bool IsDirty { get; set; }

        /// <summary>
        /// This method should be used in a Property's Set Accessor as it sets the property value and triggers the <see cref="RaisePropertyChanged(string)"/>. Plus it sets the <see cref="IsDirty"/> property.
        /// <para/>
        /// For Example:
        /// <code>
        ///string _firstName = string.Empty; //the back property.
        ///
        ///[Field]
        ///public string FirstName { get => _firstName; set => UpdateProperty(ref value, ref _firstName); }
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">the value keyword provided by the Set Accessor</param>
        /// <param name="_backProp">The back property used in the Accessors</param>
        /// <param name="propName">The Property's name</param>
        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "");

        /// <summary>
        /// It triggers the PropertyChanged event which tells the GUI to update.
        /// </summary>
        /// <param name="propName">The name of the property</param>
        public void RaisePropertyChanged(string propName);

        /// <summary>
        /// This event is triggered once a property, whose Set Accessor uses <see cref="UpdateProperty{T}(ref T?, ref T?, string)"/>, has been updated.
        /// </summary>
        public event AfterUpdateEventHandler? AfterUpdate;

        /// <summary>
        /// This event is triggered before a property, whose Set Accessor uses <see cref="UpdateProperty{T}(ref T?, ref T?, string)"/>, has been updated.
        /// </summary>
        public event BeforeUpdateEventHandler? BeforeUpdate;

    }

}
