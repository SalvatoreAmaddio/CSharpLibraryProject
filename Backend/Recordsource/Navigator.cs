using Backend.Model;
using System.Collections;

namespace Backend.Recordsource
{
    /// <summary>
    /// Concrete implementation of the <see cref="INavigator"/> Interface.
    /// </summary>
    public class Navigator : INavigator
    {
        private ISQLModel[] _records;
        public int Index { get; set; } = -1;
        public int RecordCount => _records.Length;
        public bool IsNewRecord => Index > LastIndex;
        public bool IsEmpty => RecordCount == 0;
        public bool BOF => Index == 0;
        public bool EOF => Index == LastIndex;
        public bool NoRecords => !IsNewRecord && IsEmpty;
        public int RecNum => Index + 1;
        object? IEnumerator.Current => Current;

        /// <summary>
        /// The last zero-based position of the array.
        /// </summary>
        /// <value>An integer telling which is the last position in the array.</value>
        private int LastIndex => (IsEmpty) ? -1 : RecordCount - 1;

        public Navigator(IEnumerable<ISQLModel> source)
        {
            _records = source.ToArray();
            if (IsEmpty) Index = -1;
        }

        /// <summary>
        /// Gets the record in the array at the current position within the array.
        /// </summary>
        /// <value>The element in the collection at the current position of the enumerator. 
        /// <para>Returns null if the current position is poiting to a New Record</para></value>
        public ISQLModel? Current
        {
            get
            {
                try
                {
                    return _records[Index];
                }
                catch (Exception ex) when (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
                {
                    if (IsEmpty || IsNewRecord)
                        return null;
                    else
                    {
                        MoveFirst();
                        return _records[Index];
                    }
                }
            }
        }

        public void Dispose()
        {
            _records = null!;
            GC.SuppressFinalize(this);
        }
        public bool MoveNext()
        {
            Index = IsEmpty ? -1 : ++Index;
            return Index <= LastIndex;
        }
        public bool MovePrevious()
        {
            Index = IsEmpty ? -1 : --Index;
            return Index > -1;
        }
        public bool MoveFirst()
        {
            Index = (IsEmpty) ? -1 : 0;
            return RecordCount > 0;
        }
        public bool MoveLast()
        {
            Index = (IsEmpty) ? -1 : LastIndex;
            return RecordCount > 0;
        }
        public bool MoveNew()
        {
            Index = RecordCount;
            return true;
        }
        public bool MoveAt(int index)
        {
            Index = (IsEmpty) ? -1 : index;
            return Index <= LastIndex;
        }

        public bool MoveAt(ISQLModel record)
        {
            for (int i = 0; i < RecordCount; i++)
            {
                if (_records[i].Equals(record))
                {
                    Index = i;
                    return true;
                }
            }
            return false;
        }

        public void Reset() => Index = 0;

        public override string? ToString() =>
        $"Count: {RecordCount}; Record Number: {RecNum}; BOF: {BOF}; EOF: {EOF}; NewRecord: {IsNewRecord}; No Records: {NoRecords}";
    }

}
