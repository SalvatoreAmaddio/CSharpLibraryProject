using Backend.Controller;
using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using MvvmHelpers;

namespace Backend.Recordsource
{

    /// <summary>
    /// This class extends the <see cref="ObservableRangeCollection{T}"/> and deals with IEnumerable&lt;<see cref="ISQLModel"/>&gt;. As Enumerator it uses a <see cref="ISourceNavigator"/>.
    /// see also the <seealso cref="SourceNavigator"/> class.
    /// </summary>
    public class RecordSource : ObservableRangeCollection<ISQLModel>, IParentSource, IChildSource
    {
        private ISourceNavigator? navigator;
        protected List<IChildSource> Children { get; } = [];
        public IParentSource? ParentSource { get; set; }

        /// <summary>
        /// The Controller to which this RecordSource is associated to.
        /// </summary>
        public IAbstractSQLModelController? Controller { get; set; }

        /// <summary>
        /// Parameterless Constructor to instantiate a RecordSource object.
        /// </summary>
        public RecordSource() { }

        /// <summary>
        /// It instantiates a RecordSource object filled with the given IEnumerable&lt;<see cref="ISQLModel"/>&gt;.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;<see cref="ISQLModel"/>&gt;</param>
        public RecordSource(IEnumerable<ISQLModel> source) : base(source) { }

        /// <summary>
        /// Override the default <c>GetEnumerator()</c> method to replace it with a <see cref="ISourceNavigator"></see> object./>
        /// </summary>
        /// <returns>An Enumerator object.</returns>
        public new IEnumerator<ISQLModel> GetEnumerator()
        {
            if (navigator != null) 
            {
                navigator = new SourceNavigator(this, navigator.Index);
                return navigator!;
            }
            navigator = new SourceNavigator(this);
            return navigator!;
        }

        /// <summary>
        /// Return the Enumerator as an <see cref="ISourceNavigator"/> object.
        /// </summary>
        /// <returns>A <see cref="ISourceNavigator"/> object.</returns>
        public ISourceNavigator Navigate() => (ISourceNavigator)GetEnumerator();

        /// <summary>
        /// It takes an IAsyncEnumerable, converts it to a List and returns a RecordSource object.
        /// </summary>
        /// <param name="source"> An IAsyncEnumerable&lt;ISQLModel></param>
        /// <returns>Task&lt;RecordSource></returns>
        public static async Task<RecordSource> CreateFromAsyncList(IAsyncEnumerable<ISQLModel> source) =>
        new RecordSource(await source.ToListAsync());

        /// <summary>
        /// Return a the position of the records within the RecordSource.
        /// </summary>
        /// <returns>A string.</returns>
        public virtual string RecordPositionDisplayer()
        {
            if (Navigate() == null) throw new NoNavigatorException();
            return true switch
            {
                true when Navigate().NoRecords => "NO RECORDS",
                true when Navigate().IsNewRecord => "New Record",
                _ => $"Record {Navigate()?.RecNum} of {Navigate()?.RecordCount}",
            };
        }

        public void AddChild(IChildSource child) 
        {
            child.ParentSource = this;
            Children.Add(child);
        }    

        public void NotifyChildren(CRUD crud, ISQLModel model)
        {
            if (crud == CRUD.UPDATE) return;
            foreach (IChildSource child in Children) child.Update(crud, model);
        }

        public virtual void Update(CRUD crud, ISQLModel model)
        {
            switch (crud)
            {
                case CRUD.INSERT:
                    Add(model);
                    Controller?.GoLast();
                    break;
              //case CRUD.UPDATE: NO NEEDED BECAUSE OBJECTS ARE REFERENCED.
                //  break;
                case CRUD.DELETE:
                    bool removed = Remove(model);
                    if (!removed) break;
                    if (navigator == null) throw new NoNavigatorException();
                    if (navigator.BOF && !navigator.NoRecords) Controller?.GoFirst();
                    else Controller?.GoPrevious();
                    break;
            }
        }

        public void RemoveChild(IChildSource child) => Children.Remove(child);

        public void ReplaceRecords(IEnumerable<ISQLModel> range) => ReplaceRange(range);

    }
}