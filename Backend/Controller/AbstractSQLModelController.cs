using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Recordsource;

namespace Backend.Controller
{
    public abstract class AbstractSQLModelController : AbstractNotifier, IAbstractSQLModelController
    {
        public IAbstractDatabase Db { get; protected set; } = null!;
        protected ISQLModel? _currentModel;
        private string _records = string.Empty;
        private bool _allowNewRecord = true;
        public abstract int DatabaseIndex { get; }
        public bool AllowNewRecord { get => _allowNewRecord; set => UpdateProperty(ref value, ref _allowNewRecord); }
        public RecordSource Source { get; protected set; } = null!;
        protected INavigator Navigator => Source.Navigate();
        public virtual ISQLModel? CurrentModel { get => _currentModel; set => UpdateProperty(ref value, ref _currentModel); }
        public string Records { get => _records; protected set => UpdateProperty(ref value, ref _records); }
        public AbstractSQLModelController()
        {
            Db = DatabaseManager.Do[DatabaseIndex];
            if (Db.Records == null) throw new Exception($"{Db} has no records");
            Source = new(Db.Records);
            Source.Controller = this;
            Db.Records.AddChild(Source);
            GoFirst();
        }

        public void GoNext()
        {
            Navigator.MoveNext();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoPrevious()
        {
            Navigator.MovePrevious();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoLast()
        {
            Navigator.MoveLast();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoFirst()
        {
            Navigator.MoveFirst();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoNew()
        {
            if (!AllowNewRecord) return;
            Navigator.MoveNew();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoAt(int index)
        {
            Navigator.MoveAt(index);
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoAt(ISQLModel? record)
        {
            if (record == null) CurrentModel = null;
            else if (record.IsNewRecord()) GoNew();
            else
            {
                Navigator.MoveAt(record);
                CurrentModel = Navigator.Current;
                Records = Source.RecordPositionDisplayer();
            }
        }

        public void DeleteRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentModel == null) throw new NoModelException();
            Db.Model = CurrentModel;
            Db.Crud(CRUD.DELETE, sql, parameters);
            Db?.Records?.Remove(Db.Model);
            Db?.Records?.NotifyChildren(CRUD.DELETE, Db.Model);
            if (Navigator.BOF && !Navigator.NoRecords) GoFirst();
            else GoPrevious();
        }

        public void AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentModel == null) throw new NoModelException();
            if (!CurrentModel.IsDirty) return;
            Db.Model = CurrentModel;
            CRUD crud = (!Db.Model.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            Db.Crud(crud, sql, parameters);
            Db.Model.IsDirty = false;
            Db.Records?.NotifyChildren(crud, Db.Model);
            GoAt(CurrentModel);
        }
    }
}