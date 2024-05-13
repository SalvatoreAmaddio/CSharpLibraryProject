using Backend.Controller;
using Backend.Database;
using Backend.Events;
using Backend.Exceptions;
using Backend.Model;
using MvvmHelpers;

namespace Backend.Recordsource
{
    /// <summary>
    /// This class extends the <see cref="ObservableRangeCollection{T}"/> and deals with IEnumerable&lt;<see cref="ISQLModel"/>&gt;. As Enumerator it uses a <see cref="INavigator"/>.
    /// see also the <seealso cref="Navigator"/> class.
    /// </summary>
    public class RecordSource : ObservableRangeCollection<ISQLModel>, IParentSource, IChildSource
    {
        private INavigator? navigator;
        private List<IChildSource> Children { get; } = [];
        
        /// <summary>
        /// The Controller to which this RecordSource is associated to.
        /// </summary>
        public IAbstractSQLModelController? Controller { get; set; }

        /// <summary>
        /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and this <see cref="RecordSource"/>.
        /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
        /// </summary>
        public event FilterEventHandler? RunFilter;

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
        /// Override the default <c>GetEnumerator()</c> method to replace it with a <see cref="INavigator"></see> object./>
        /// </summary>
        /// <returns>An Enumerator object.</returns>
        public new IEnumerator<ISQLModel> GetEnumerator()
        {
            if (navigator != null) 
            {
                navigator = new Navigator(this, navigator.Index);
                return navigator!;
            }

            navigator = new Navigator(this);
            return navigator!;
        }

        /// <summary>
        /// Return the Enumerator as an <see cref="INavigator"/> object.
        /// </summary>
        /// <returns>A <see cref="INavigator"/> object.</returns>
        public INavigator Navigate() => (INavigator)GetEnumerator();

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
        public string RecordPositionDisplayer()
        {
            if (navigator == null) throw new NoNavigatorException();
            return true switch
            {
                true when navigator.NoRecords => "NO RECORDS",
                true when navigator.IsNewRecord => "New Record",
                _ => $"Record {navigator?.RecNum} of {navigator?.RecordCount}",
            };
        }

        public void AddChild(IChildSource child) => Children.Add(child);   

        public void NotifyChildren(CRUD crud, ISQLModel model)
        {
            if (crud == CRUD.UPDATE) return;
            foreach (var child in Children) child.Update(crud, model);
        }

        public void Update(CRUD crud, ISQLModel model)
        {
            switch (crud)
            {
                case CRUD.INSERT:
                    if (Controller!.VoidParentUpdate) return;
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
                    if (Controller!.VoidParentUpdate) return;
                    break;
            }
            RunFilter?.Invoke(this, new());
        }

        public void RemoveChild(IChildSource child) => Children.Remove(child);
    }
}