using Backend.Database;
using Backend.Model;
using Backend.Recordsource;
using FrontEnd.Events;
using FrontEnd.Model;

namespace FrontEnd.Source
{
    public interface IRecordSource<M> : IRecordSource where M : AbstractModel, new()
    {
        /// <summary>
        /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and this <see cref="RecordSource"/>.
        /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
        /// </summary>
        public event FilterEventHandler? RunFilter;

        public void ReplaceRecords(IEnumerable<ISQLModel> range);

    }

    public class RecordSource<M> : RecordSource, IRecordSource<M> where M : AbstractModel, new()
    {
        public event FilterEventHandler? RunFilter;

        /// <summary>
        /// It instantiates a RecordSource object filled with the given IEnumerable&lt;<see cref="ISQLModel"/>&gt;.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;<see cref="ISQLModel"/>&gt;</param>
        public RecordSource(IEnumerable<ISQLModel> source) : base(source) { }

        public RecordSource() { }

        public override void Update(CRUD crud, ISQLModel model)
        {
            if (crud == CRUD.INSERT && Controller!.VoidParentUpdate) return;

            base.Update(crud, model);

            if (Controller!.VoidParentUpdate) return;
            RunFilter?.Invoke(this, new());
        }

        public void ReplaceRecords(IEnumerable<ISQLModel> range)
        {
            ReplaceRange(range);
        }
    }
}
