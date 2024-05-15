using Backend.Database;
using Backend.Model;
using Backend.Recordsource;
using FrontEnd.Events;
using FrontEnd.Model;

namespace FrontEnd.Recordsource
{

    public class RecordSource<M> : RecordSource where M : AbstractModel, new()
    {
        /// <summary>
        /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and this <see cref="Backend.Recordsource.RecordSource"/>.
        /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
        /// </summary>
        public event FilterEventHandler? RunFilter;

        public RecordSource(IEnumerable<ISQLModel> source) : base(source) { }

        public override void Update(CRUD crud, ISQLModel model)
        {
            if (crud == CRUD.INSERT && Controller!.VoidParentUpdate) return;

            base.Update(crud, model);

            if (Controller!.VoidParentUpdate) return;
            RunFilter?.Invoke(this, new());
        }
    }
}