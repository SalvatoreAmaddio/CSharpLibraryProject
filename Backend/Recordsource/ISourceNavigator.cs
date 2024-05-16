using Backend.Model;

namespace Backend.Recordsource
{
    public interface INavigator 
    {
        /// <summary>
        /// Tells if there are no records at all. 
        /// This property returns false if <see cref="Index"/> is pointing to a New Record.<para/>
        /// see also: <seealso cref="IsNewRecord"/>
        /// </summary>
        public bool NoRecords { get; }

        /// <summary>
        /// Gets and Sets the zero-based position within the array. Default Value is: -1;
        /// </summary>
        /// <value>The current zero-based position within the array.</value>
        public int Index { get; set; }

        /// <summary>
        /// Tells if the collection is empty.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Tells if <see cref="Index"/> is at the beggining of the Collection.
        /// </summary>
        public bool BOF { get; }

        /// <summary>
        /// Tells if <see cref="Index"/> is at the end of the Collection.
        /// </summary>
        public bool EOF { get; }

        /// <summary>
        /// Tells if Enumerator is pointing at a New Record.
        /// </summary>
        public bool IsNewRecord { get; }

        /// <summary>
        /// Get the one-based position within the Collection.
        /// see <see cref="Index"/>.
        /// <example>
        ///  <para>For example: array[0] is record number 1, array[1] is record number 2 ...</para>
        /// </example>
        /// </summary>
        public int RecNum { get; }

        /// <summary>
        /// Moves the Enumerator to the previous element in the collection.
        /// </summary>
        /// <returns>
        /// true if the Enumerator did move.
        /// </returns>
        public bool GoPrevious();

        /// <summary>
        /// Moves the Enumerator to the first element in the collection.
        /// </summary>
        /// <returns>
        /// true if the Enumerator could move.
        /// </returns>
        public bool GoFirst();

        /// <summary>
        /// Moves the Enumerator to the last element in the collection.
        /// </summary>
        /// <returns>
        /// true if the Enumerator could move.
        /// </returns>
        public bool GoLast();

        /// <summary>
        /// Moves the Enumerator beyond the last element in the collection indicating a new Record can be added.
        /// </summary>
        /// <returns>
        /// true if the Enumerator could move.
        /// </returns>
        public bool GoNew();

        /// <summary>
        /// Moves the Enumerator to the element at the given position in collection.
        /// </summary>
        /// <param name="index">the zero-based position to move the Enumerator at.</param>
        /// <returns>
        /// true if the Enumerator could move.
        /// </returns>
        public bool GoAt(int index);

        /// <summary>
        /// Moves the Enumerator to the given element in collection.
        /// </summary>
        /// <param name="record">A ISQLModel</param>
        /// <returns>
        /// true if the Enumerator could move.
        /// </returns>
        public bool GoAt(object record);

        /// <summary>
        /// Tells how many records are in the collection.
        /// </summary>
        /// <value>The number of records in collection</value>
        public int RecordCount { get; }

        public bool GoNext();

        public M? CurrentRecord<M>();
    }

    /// <summary>
    /// This interface extends the IEnumerator&lt;ISQLModel&gt; 
    /// to add extra functionalities.
    /// For instance, this enumerator can move up and down the IEnumerable.
    /// This interface is meant for dealing with IEnumerable&lt;<see cref="ISQLModel"/>&gt; only.
    /// </summary>
    public interface ISourceNavigator : IEnumerator<ISQLModel?>, INavigator { }
}
