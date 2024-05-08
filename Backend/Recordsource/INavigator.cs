using Backend.Model;

namespace Backend.Recordsource
{
    /// <summary>
    /// This interface extends the IEnumerator&lt;ISQLModel&gt; 
    /// and adds extra functionalities.
    /// For instance, this enumerator can move up and down the IEnumerable.
    /// This interface is meant for dealing with IEnumerable&lt;<see cref="ISQLModel"/>&gt; only.
    /// </summary>
    public interface INavigator : IEnumerator<ISQLModel?>
    {
        /// <summary>
        /// This boolean indicates if there are no records at all. 
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
        /// It tells if the Array is empty.
        /// </summary>
        /// <value>True if the Array is empty</value>
        public bool IsEmpty { get; }

        /// <summary>
        /// This boolean indicates if <see cref="Index"/> is at the beggining of the Array.
        /// </summary>
        /// <value>True if <see cref="Index"/> is at the first position. i.e. 0</value>
        public bool BOF { get; }

        /// <summary>
        /// This boolean indicates if <see cref="Index"/> is at the end of the Array. Therefore, it reached the last available position.
        /// </summary>
        /// <value>True if <see cref="Index"/> is at the last position.</value>
        public bool EOF { get; }

        /// <summary>
        /// This boolean indicates if Enumerator is pointing to a New Record.
        /// </summary>
        /// <value>True if it is pointing a New Record</value>
        public bool IsNewRecord { get; }

        /// <summary>
        /// Get the one-based position within the Array.
        /// see <see cref="Index"/>.
        /// <example>
        ///  <para>For example: array[0] is record number 1, array[1] is record number 2 ...</para>
        /// </example>
        /// </summary>
        /// <value>The Record's Number.</value>
        public int RecNum { get; }

        /// <summary>
        /// It moves the Enumerator to the previous position.
        /// </summary>
        /// <returns>
        /// True if the Enumerator could move.
        /// </returns>
        public bool MovePrevious();

        /// <summary>
        /// It moves the Enumerator to the next position.
        /// </summary>
        /// <returns>
        /// True if the Enumerator could move.
        /// </returns>
        public bool MoveFirst();

        /// <summary>
        /// It moves the Enumerator to the last position.
        /// </summary>
        /// <returns>
        /// True if the Enumerator could move.
        /// </returns>
        public bool MoveLast();

        /// <summary>
        /// It moves the Enumerator beyond the last position indicating a new Record can be added.
        /// </summary>
        /// <returns>
        /// True if the Enumerator could move.
        /// </returns>
        public bool MoveNew();

        /// <summary>
        /// It moves the Enumerator at a given position in the array.
        /// </summary>
        /// <param name="index">the zero-based position to move the Enumerator at.</param>
        /// <returns>
        /// True if the Enumerator could move.
        /// </returns>
        public bool MoveAt(int index);

        /// <summary>
        /// It finds the record and moves the Enumerator to its position in the array.
        /// </summary>
        /// <param name="record">A ISQLModel</param>
        /// <returns>
        /// True if the Enumerator could move.
        /// </returns>
        public bool MoveAt(ISQLModel record);

        /// <summary>
        /// It tells how many records are in the array.
        /// </summary>
        /// <value>The number of records in the array</value>
        public int RecordCount { get; }

    }
}
