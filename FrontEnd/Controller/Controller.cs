using Backend.Controller;
using Backend.Database;
using Backend.Model;
using Backend.Recordsource;
using FrontEnd.FilterSource;
using System.Windows;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    public interface IAbstractController : IAbstractSQLModelController
    {
        public bool IsLoading { get; set; }

    }
    public interface IAbstractFormController<M> : IAbstractController where M : ISQLModel, new()
    {
        /// <summary>
        /// A more concrete version of <see cref="IAbstractSQLModelController.CurrentModel"/>
        /// </summary>
        /// <value>The actual object that implements <see cref="IAbstractSQLModelController.CurrentModel"/></value>
        public M? CurrentRecord { get; set; }
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
    }

    public interface IAbstractFormListController<M> : IAbstractFormController<M> where M : ISQLModel, new()
    {
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public string Search { get; set; }
    }
    
    public interface IListController 
    {
        public void Filter(OnSelectedEventArgs e);

        /// <summary>
        /// Gets and Sets the default Search Query to be used. This property works in conjunction with a <see cref="FilterQueryBuilder"/> object.
        /// <para/>
        /// Your statement must have a WHERE clause.
        /// </summary>
        public string DefaultSearchQry { get; set; }

    }

    public abstract class AbstractController<M> : AbstractSQLModelController, IAbstractFormController<M> where M : ISQLModel, new()
    {
        string _search = string.Empty;
        bool _isloading = false;
        public bool IsLoading { get => _isloading; set => UpdateProperty(ref value, ref _isloading); }
        public string Search { get => _search; set => UpdateProperty(ref value, ref _search); }
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
        public M? CurrentRecord
        {
            get => (M?)CurrentModel;
            set => CurrentModel = value;
        }
        public override ISQLModel? CurrentModel
        {
            get => base.CurrentModel;
            set
            {
                base.CurrentModel = value;
                RaisePropertyChanged(nameof(CurrentRecord));
            }
        }
        public AbstractController() : base()
        {
            UpdateCMD = new CMD<M>(Update);
            DeleteCMD = new CMD<M>(Delete);
        }

        protected virtual void Update(M? model)
        {
            if (model == null) return;
            CurrentRecord = model;
            AlterRecord();
        }
        protected virtual void Delete(M? model)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this record?","Confirm",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;
            CurrentRecord = model;
            DeleteRecord();
        }

        public override void GoNew()
        {
            if (!AllowNewRecord) return;
            _navigator.MoveNew();
            CurrentRecord = new M();
            Records = Source.RecordPositionDisplayer();
        }
    }

    public abstract class AbstractListController<M> : AbstractController<M>, IListController, IAbstractFormListController<M> where M : ISQLModel, new()
    {
        protected FilterQueryBuilder QueryBuiler;
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public abstract string DefaultSearchQry { get; set; }
        protected abstract void Open(M? model);
        protected abstract void OpenNew(M? model);
        protected readonly List<QueryParameter> SearchParameters = [];
        public AbstractListController() : base()
        {
            OpenCMD = new CMD<M>(Open);
            OpenNewCMD = new CMD<M>(OpenNew);
            QueryBuiler = new(DefaultSearchQry);
        }

        public override void GoNew()
        {
            base.GoNew();
            OpenNew(CurrentRecord);
        }
        abstract public void Filter(OnSelectedEventArgs e);

        /// <summary>
        /// Wrap up method for the <see cref="RecordSource.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/>
        /// </summary>
        /// <param name="qry">The query to be used, can be null</param>
        /// <param name="parameters">A list of parameters to be used, can be null</param>
        /// <returns>A RecordSource</returns>
        public Task<RecordSource> CreateFromAsyncList(string? qry = null, List<QueryParameter>? parameters = null) => RecordSource.CreateFromAsyncList(Db.RetrieveAsync(qry, parameters));
    }

    /// <summary>
    /// This class helps build the Search Query and the parameters to be used when searching the recordset. 
    /// <para/>
    /// <c>IMPORTANT:</c>
    /// This class is not designed to handle Aggregation Queries.
    /// </summary>
    /// <param name="defaultQuery">The Default Query Structure; This will tipically be the <see cref="AbstractListController{M}.DefaultSearchQry"/> property.</param>
    public class FilterQueryBuilder(string defaultQuery)
    {
        private readonly string _defaultQuery = defaultQuery;
        private string _qry = string.Empty;
        private readonly List<QueryParameter> _parameters = [];
        
        /// <summary>
        /// Gets the List of parameters to be used in the Search Query.
        /// </summary>
        public List<QueryParameter> Params => _parameters;

        /// <summary>
        /// The final query to be used in the Search.
        /// </summary>
        public string Query { get => string.IsNullOrEmpty(_qry) ? _defaultQuery : _qry; }
        
        /// <summary>
        /// Reset the object to its initial state.
        /// </summary>
        public void Clear() 
        {
            _qry = string.Empty;
            _parameters.Clear();
        }
        
        /// <summary>
        /// Add a parameter to the <see cref="Params"/> property.
        /// </summary>
        /// <param name="placeholder">A string representing the placeholder. e.g. @name</param>
        /// <param name="value">The value of the parameter</param>
        public void AddParameter(string placeholder, object? value) => _parameters.Add(new(placeholder,value));
        
        /// <summary>
        /// Add a condition to the WHERE clause to the Default Query. <para/>
        /// </summary>
        /// <param name="condition"></param>
        public void AddCondition(string condition) 
        {
            if (condition.Length > 0) 
            {
                if (string.IsNullOrEmpty(_qry)) 
                    _qry = _defaultQuery;

                _qry += $" AND ({condition})";
            }
        }
    }
}
