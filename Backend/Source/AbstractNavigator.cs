namespace Backend.Source
{
    public abstract class AbstractNavigator : INavigator
    {
        public int Index { get; set; } = -1;
        public virtual int RecordCount { get; }
        public bool IsNewRecord => Index > LastIndex;
        public bool IsEmpty => RecordCount == 0;
        public bool BOF => Index == 0;
        public bool EOF => Index == LastIndex;
        public bool NoRecords => !IsNewRecord && IsEmpty;
        public int RecNum => Index + 1;

        /// <summary>
        /// The last zero-based position of the array.
        /// </summary>
        /// <value>An integer telling which is the last position in the array.</value>
        protected int LastIndex => (IsEmpty) ? -1 : RecordCount - 1;

        public void Dispose()
        {
            ClearArray();
            GC.SuppressFinalize(this);
        }
        public bool MoveNext()
        {
            Index = IsEmpty ? -1 : ++Index;
            return Index <= LastIndex;
        }
        public bool GoPrevious()
        {
            Index = IsEmpty ? -1 : --Index;
            return Index > -1;
        }
        public bool GoFirst()
        {
            Index = (IsEmpty) ? -1 : 0;
            return RecordCount > 0;
        }
        public bool GoLast()
        {
            Index = (IsEmpty) ? -1 : LastIndex;
            return RecordCount > 0;
        }
        public bool GoNew()
        {
            Index = RecordCount;
            return true;
        }
        public bool GoAt(int index)
        {
            Index = (IsEmpty) ? -1 : index;
            return Index <= LastIndex;
        }
        public void Reset() => Index = 0;
        public override string? ToString() => $"Count: {RecordCount}; Record Number: {RecNum}; BOF: {BOF}; EOF: {EOF}; NewRecord: {IsNewRecord}; No Records: {NoRecords}";
        protected abstract void ClearArray();
        protected abstract object GetObject(int index);

        public M? CurrentRecord<M>()
        {
            try
            {
                return (M?)GetObject(Index);
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
            {
                if (IsEmpty || IsNewRecord)
                    return default;
                else
                {
                    GoFirst();
                    return (M?)GetObject(Index);
                }
            }
        }

        public bool GoAt(object record)
        {
            for (int i = 0; i < RecordCount; i++)
            {
                if (GetObject(i).Equals(record))
                {
                    Index = i;
                    return true;
                }
            }
            return false;
        }

        public bool GoNext() => MoveNext();
    }

}
