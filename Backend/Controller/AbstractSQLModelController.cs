using Backend.Database;
using Backend.Events;
using Backend.Exceptions;
using Backend.Model;
using Backend.Recordsource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        protected INavigator _navigator => Source.Navigate();
        public virtual ISQLModel? CurrentModel { get => _currentModel; set => UpdateProperty(ref value, ref _currentModel); }
        public string Records { get => _records; protected set => UpdateProperty(ref value, ref _records); }
        public AbstractSQLModelController()
        {
            Db = DatabaseManager.Do[DatabaseIndex];
            if (Db.Records == null) throw new Exception($"{Db} has no records");
            Source = new(Db.Records);
            Db.Records.AddChild(Source);
            GoFirst();
        }

        public void GoNext()
        {
            _navigator.MoveNext();
            CurrentModel = _navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoPrevious()
        {
            _navigator.MovePrevious();
            CurrentModel = _navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoLast()
        {
            _navigator.MoveLast();
            CurrentModel = _navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoFirst()
        {
            _navigator.MoveFirst();
            CurrentModel = _navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoNew()
        {
            if (!AllowNewRecord) return;
            _navigator.MoveNew();
            CurrentModel = _navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoAt(int index)
        {
            _navigator.MoveAt(index);
            CurrentModel = _navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public void GoAt(ISQLModel? record)
        {
            if (record == null) CurrentModel = null;
            else
            {
                _navigator.MoveAt(record);
                CurrentModel = _navigator.Current;
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
            if (_navigator.BOF && !_navigator.NoRecords) GoFirst();
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