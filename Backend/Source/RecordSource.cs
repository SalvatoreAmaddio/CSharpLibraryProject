using Backend.Controller;
using Backend.Database;
using Backend.Events;
using Backend.Exceptions;
using Backend.Model;
using MvvmHelpers;

namespace Backend.Source
{
    /// <summary>
    /// This interface serves as a bridge between a <see cref="RecordSource"/> object and a Combo object in the contest of GUI Development.
    /// The main purpose of this interface is to update the ComboBox control to reflect changes that occured in the ComboBox's ItemSource which is an instance of <see cref="RecordSource"/>
    /// <para/>
    /// Indeed, the <see cref="OnItemSourceUpdated"/> is called in <see cref="RecordSource.Update(CRUD, ISQLModel)"/>.
    /// </summary>
    public interface IUIControl 
    {
        /// <summary>
        /// Whenever the Parent RecordSource is updated, it notifies this object to reflect the update.
        /// </summary>
        public void OnItemSourceUpdated(); 
       
    }

    /// <summary>
    /// This class extends the <see cref="ObservableRangeCollection{T}"/> and deals with IEnumerable&lt;<see cref="ISQLModel"/>&gt;. As Enumerator it uses a <see cref="ISourceNavigator"/>.
    /// see also the <seealso cref="Navigator"/> class.
    /// </summary>
    public class RecordSource : ObservableRangeCollection<ISQLModel>, IParentSource, IChildSource, IDisposable
    {
        /// <summary>
        /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and this <see cref="Backend.Source.RecordSource"/>.
        /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
        /// </summary>
        public event FilterEventHandler? RunFilter;

        private INavigator? navigator;
        private List<IChildSource> Children { get; } = [];
        public IParentSource? ParentSource { get; set; }

        /// <summary>
        /// The Controller to which this RecordSource is associated to.
        /// </summary>
        public IAbstractSQLModelController? Controller { get; set; }

        /// <summary>
        /// A list of <see cref="IUIControl"/> associated to this <see cref="RecordSource"/>.
        /// </summary>
        private List<IUIControl>? UIControls;

        #region Constructor
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
        /// It instantiates a RecordSource object filled with the given <see cref="IAbstractDatabase.Records"/> IEnumerable.
        /// This constructor will consider this RecordSource object as a child of the <see cref="IAbstractDatabase.Records"/>
        /// </summary>
        /// <param name="db">An instance of <see cref="IAbstractDatabase"/></param>
        public RecordSource(IAbstractDatabase db) : this(db.Records) => db.Records.AddChild(this);

        /// <summary>
        /// It instantiates a RecordSource object filled with the given <see cref="IAbstractDatabase.Records"/> IEnumerable.
        /// This constructor will consider this RecordSource object as a child of the <see cref="IAbstractDatabase.Records"/>
        /// </summary>
        /// <param name="db">An instance of <see cref="IAbstractDatabase"/></param>
        /// <param name="controller">An instance of <see cref="IAbstractSQLModelController"/></param>
        public RecordSource(IAbstractDatabase db, IAbstractSQLModelController controller) : this(db) => Controller = controller;
        #endregion

        #region Enumerator
        /// <summary>
        /// Override the default <c>GetEnumerator()</c> method to replace it with a <see cref="ISourceNavigator"></see> object./>
        /// </summary>
        /// <returns>An Enumerator object.</returns>
        public new IEnumerator<ISQLModel?> GetEnumerator()
        {
            if (navigator != null)
            {
                navigator = new Navigator(this, navigator.Index, navigator.AllowNewRecord);
                return navigator;
            }
            navigator = new Navigator(this);
            return navigator;
        }

        /// <summary>
        /// Return the Enumerator as an <see cref="INavigator"/> object.
        /// </summary>
        /// <returns>A <see cref="INavigator"/> object.</returns>
        public INavigator Navigate() => (INavigator)GetEnumerator();
        #endregion

        #region ObserverPattern
        public void AddChild(IChildSource child)
        {
            child.ParentSource = this;
            Children.Add(child);
        }

        public void NotifyChildren(CRUD crud, ISQLModel model)
        {
            foreach (IChildSource child in Children) child.Update(crud, model);
        }

        public virtual void Update(CRUD crud, ISQLModel model)
        {
            switch (crud)
            {
                case CRUD.INSERT:
                    if (Controller != null && Controller.VoidParentUpdate) return;
                    Add(model);
                    Controller?.GoLast();
                    break;
                case CRUD.UPDATE:
                    NotifyUIControl();
                    break;
                case CRUD.DELETE:
                    bool removed = Remove(model);
                    if (!removed) break;
                    if (navigator != null) 
                    {
                        if (navigator.BOF && !navigator.NoRecords) Controller?.GoFirst();
                        else Controller?.GoPrevious();
                    }
                    break;
            }

            if (Controller != null && Controller.VoidParentUpdate) return;
            RunFilter?.Invoke(this, new());
        }

        /// <summary>
        /// It adds a <see cref="IUIControl"/> object to the <see cref="UIControls"/>.
        /// <para/>
        /// If <see cref="UIControls"/> is null, it gets initialised.
        /// </summary>
        /// <param name="control">An object implementing <see cref="IUIControl"/></param>
        public void AddUIControlReference(IUIControl control) 
        {
            if (UIControls == null) UIControls = [];
            UIControls.Add(control);
        }

        /// <summary>
        /// This method is called in <see cref="Update(CRUD, ISQLModel)"/>.
        /// It loops through the <see cref="UIControls"/> to notify the <see cref="IUIControl"/> object to reflect changes that occured to their ItemsSource which is an instance of <see cref="RecordSource"/>.
        /// </summary>
        public void NotifyUIControl()
        {
            if (UIControls != null && UIControls.Count > 0)
                foreach (IUIControl combo in UIControls) combo.OnItemSourceUpdated();
        }

        public void RemoveChild(IChildSource child) => Children.Remove(child);
        #endregion

        public void ReplaceRecords(IEnumerable<ISQLModel> range) => ReplaceRange(range);

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
            if (navigator == null) throw new NoNavigatorException();
            return true switch
            {
                true when navigator.NoRecords => "NO RECORDS",
                true when navigator.IsNewRecord => "New Record",
                _ => $"Record {navigator?.RecNum} of {navigator?.RecordCount}",
            };
        }

        public void Dispose()
        {
            ParentSource?.RemoveChild(this);
            UIControls?.Clear();
            Children.Clear();
            navigator = null;
            RunFilter = null;
            GC.SuppressFinalize(this);
        }

        ~RecordSource()
        {
            Dispose();
        }
    }
}