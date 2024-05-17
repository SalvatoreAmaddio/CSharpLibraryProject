using Backend.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
using FrontEnd.Notifier;
using System.Windows.Input;
using FrontEnd.Forms.FormComponents;
using FrontEnd.Forms;
using System.ComponentModel;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This interface defines a set of methods and properties to work as a bridge between an instances of <see cref="IAbstractFormController"/>, <see cref="IAbstractFormListController"/>  and <see cref="SubForm"/> objects.
    /// </summary>
    public interface ISubFormController
    {
        /// <summary>
        /// Holds a reference to the SubForm's ParentRecord property. This property is set by the <see cref="SetParentRecord(AbstractModel?)"/>  called within the SubForm object.
        /// </summary>
        public AbstractModel? ParentRecord { get; }

        /// <summary>
        /// This method is called by the SubForm to notify its controller that the ParentController has moved to another Record.
        /// </summary>
        /// <param name="ParentRecord"></param>
        public void SetParentRecord(AbstractModel? ParentRecord);

        /// <summary>
        /// Occurs when the SubForm is going to add a new Record.
        /// <para/>
        /// For Example:
        /// <code>
        /// public YourSubFormController() => NewRecordEvent += OnNewRecordEvent;
        /// ...
        /// private void OnNewRecordEvent(object? sender, EventArgs e) 
        /// {
        ///      Employee? employee = (Employee?)ParentRecord;
        ///      if (employee!=null) 
        ///      {
        ///           CurrentRecord.Employee = new(employee.EmployeeID);
        ///           CurrentRecord.IsDirty = false;
        ///      }
        /// }
        /// </code>
        /// </summary>
        public event NewRecordEventHandler? NewRecordEvent;

        /// <summary>
        /// Override this method to implement a custom logic to filter a SubForm object.
        /// <para/>
        /// For Example:
        /// <code>
        /// ...
        /// string sql = $"SELECT * FROM YourTable WHERE YourForeignKey = @foreignKey;";
        /// List&lt;QueryParameter> queryParameters = [];
        /// queryParameters.Add(new ("employeeID", ParentRecord?.GetTablePK()?.GetValue()));
        /// var results = await CreateFromAsyncList(sql, queryParameters);
        /// Source.ReplaceRange(results);
        /// GoFirst();
        /// ...
        /// </code>
        /// </summary>
        public void OnSubFormFilter();
    }

    /// <summary>
    /// This interface extends <see cref="IAbstractSQLModelController"/> and adds properties and methods to work as a bridge between <see cref="AbstractModel"/> objects and <see cref="Form"/> objects.
    /// </summary>
    public interface IAbstractFormController : IAbstractSQLModelController, INotifier
    {
        /// <summary>
        /// Gets and Sets a boolean indicating if the Form's ProgressBar is running/> 
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Perform an Insert/Update CRUD operation on the <see cref="IAbstractSQLModelController.CurrentModel"/> property.
        /// </summary>
        /// <returns>true if the operation was successful.</returns>
        public bool PerformUpdate();

        public void OnWindowClosing(CancelEventArgs e);
    }

    /// <summary>
    /// This interface extends <see cref="IAbstractFormController"/> and adds a set of methods and properties to work as a bridge between <see cref="AbstractModel"/> objects and <see cref="FormList"/> objects.
    /// <para/>
    /// see also <seealso cref="RecordTracker"/>, <seealso cref="HeaderFilter"/>
    /// </summary>
    public interface IAbstractFormListController : IAbstractFormController
    {
        /// <summary>
        /// Override this method to implement your filter logic. 
        /// For Example:
        /// <code>
        /// //overide SearchQry Property.
        /// public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Employee)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        /// ...
        /// public override async Task SearchRecordAsync() 
        /// {
        ///     QueryBuiler.AddParameter("name", Search.ToLower() + "%");
        ///     QueryBuiler.AddParameter("name", Search.ToLower() + "%");
        ///     var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        ///     Source.ReplaceRange(results);
        ///     GoFirst();
        /// }
        /// </code>
        /// </summary>
        /// <returns>A Taks</returns>
        public Task SearchRecordAsync();

        /// <summary>
        /// This method is called by the <see cref="Forms.HeaderFilter"/> object when an option is selected or unselected.
        /// It instructs the Controller to filter its RecordSource.
        /// <para/>
        /// For Example:
        /// <code>
        /// public override async void OnOptionFilter()
        /// {
        ///     QueryBuiler.Clear();
        ///     QueryBuiler.AddCondition(GenderOptions.Conditions(QueryBuiler));
        ///     ... // Other conditions if needed
        ///     await SearchRecordAsync();
        /// }
        /// </code>
        /// </summary>
        public void OnOptionFilter();

        /// <summary>
        /// Gets and Sets the Search Query to be used. This property works in conjunction with a <see cref="FilterQueryBuilder"/> object.
        /// <para/>
        /// Your statement must have a WHERE clause.
        /// <para/>
        /// For Example:
        /// <code>
        /// public override string SearchQry { get; set; } = $"SELECT * FROM Payslip WHERE EmployeeID = @ID;";
        /// //OR
        /// public override string SearchQry { get; set; } = $"SELECT * FROM Employee WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        /// </code>
        /// </summary>
        public string SearchQry { get; set; }

        /// <summary>
        /// Tells if the Controller shall open a Window or add a new row to the <see cref="Lista"/> to add a New Record.
        /// </summary>
        public bool OpenWindowOnNew { get; set; }

        /// <summary>
        /// Removes empty new records from the Source.
        /// </summary>
        public void CleanSource();
    }

    /// <summary>
    /// This Interface extends <see cref="IAbstractFormController"/> and adds a set of ICommand properties.
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public interface IAbstractFormController<M> : IAbstractFormController where M : AbstractModel, new()
    {
        /// <summary>
        /// A more concrete version of <see cref="IAbstractSQLModelController.CurrentModel"/>
        /// </summary>
        /// <value>The actual object that implements <see cref="IAbstractSQLModelController.CurrentModel"/></value>
        public M? CurrentRecord { get; set; }
        
        /// <summary>
        /// Gets and Sets the Command to perform CRUD operations such as Insert or Update.
        /// </summary>
        public ICommand UpdateCMD { get; set; }

        /// <summary>
        /// Gets and Sets the Command to perform Delete CRUD operation.
        /// </summary>
        public ICommand DeleteCMD { get; set; }

        /// <summary>
        /// Gets and Sets the Command to requery the database table.
        /// </summary>
        public ICommand RequeryCMD { get; set; }
    }

    /// <summary>
    /// This Interface extends <see cref="IAbstractFormController{M}"/> and <see cref="IAbstractFormListController"/> and adds a set of properties necessary to deal with <see cref="FormList"/> objects.
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public interface IAbstractFormListController<M> : IAbstractFormController<M>, IAbstractFormListController where M : AbstractModel, new()
    {
        /// <summary>
        /// Gets and Sets the command to execute to open a Record.
        /// </summary>
        public ICommand OpenCMD { get; set; }

        /// <summary>
        /// Gets and Sets the command to execute to open a New Record.
        /// </summary>
        public ICommand OpenNewCMD { get; set; }

        /// <summary>
        /// Gets and Sets the string parameter used in a search textbox to filter the RecordSource.
        /// </summary>
        public string Search { get; set; }
    }
}