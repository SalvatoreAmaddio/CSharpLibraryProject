using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Recordsource;

namespace Backend.Controller
{
    public abstract class AbstractSQLModelController : IAbstractSQLModelController
    {
        public IAbstractDatabase Db { get; protected set; } = null!;

        public abstract int DatabaseIndex { get; }
        public RecordSource Source { get; protected set; } = null!;
        protected INavigator Navigator => Source.Navigate();
        public AbstractSQLModelController()
        {
            Db = DatabaseManager.Do[DatabaseIndex];
            if (Db.Records == null) throw new Exception($"{Db} has no records");
            Source = new(Db.Records);
            Source.Controller = this;
            Db.Records.AddChild(Source);
            GoFirst();
        }


        public virtual bool AllowNewRecord { get; set; }
        public virtual ISQLModel? CurrentModel { get; set; }
        public virtual string Records { get; protected set; } = string.Empty;

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
            Db?.Records?.NotifyChildren(CRUD.DELETE, Db.Model);
        }

        public virtual void AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentModel == null) throw new NoModelException();
            Db.Model = CurrentModel;
            CRUD crud = (!Db.Model.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            Db.Crud(crud, sql, parameters);
            Db.Records?.NotifyChildren(crud, Db.Model);
            GoAt(CurrentModel);
        }
    }
}