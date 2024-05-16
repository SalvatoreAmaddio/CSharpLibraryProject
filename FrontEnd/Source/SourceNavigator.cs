using Backend.Source;
using FrontEnd.Model;
using System.Collections;

namespace FrontEnd.Source
{
    public class SourceNavigator<M> : AbstractNavigator, ISourceNavigator<M> where M : AbstractModel, new()
    {
        protected M[] _records;
        public M? Current => CurrentRecord<M?>();
        object? IEnumerator.Current => Current;
        public SourceNavigator(IEnumerable<M> source)
        {
            _records = source.ToArray();
            if (IsEmpty) Index = -1;
        }
        public SourceNavigator(IEnumerable<M> source, int index) : this(source) => Index = index;
        protected override void ClearArray() => _records = null!;
        protected override object GetObject(int index) => _records[index];
    }
}