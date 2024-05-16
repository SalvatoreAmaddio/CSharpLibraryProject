using Backend.Model;
using System.Collections;

namespace Backend.Recordsource
{
    /// <summary>
    /// Concrete implementation of the <see cref="ISourceNavigator"/> Interface.
    /// </summary>
    public class SourceNavigator : AbstractNavigator, ISourceNavigator
    {
        protected ISQLModel[] _records;
        object? IEnumerator.Current => Current;
        
        /// <summary>
        /// Gets the record in the array at the current position within the array.
        /// </summary>
        /// <value>The element in the collection at the current position of the enumerator. 
        /// <para>Returns null if the current position is poiting to a New Record</para></value>
        public ISQLModel? Current
        {
            get => CurrentRecord<ISQLModel?>();
        }

        public SourceNavigator(IEnumerable<ISQLModel> source)
        {
            _records = source.ToArray();
            if (IsEmpty) Index = -1;
        }

        public SourceNavigator(IEnumerable<ISQLModel> source, int index) : this(source) => Index = index;        
        protected override void ClearArray() => _records = null!;
        protected override object GetObject(int index) => _records[index];
    }

}
