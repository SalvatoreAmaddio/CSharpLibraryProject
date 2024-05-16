using Backend.Database;
using Backend.Model;
using Backend.Recordsource;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
using FrontEnd.RecordSource;

namespace FrontEnd.Recordsource
{
    public class RecordSource<M>(IEnumerable<ISQLModel> source) : Backend.Recordsource.RecordSource(source) where M : AbstractModel, new()
    {
        /// <summary>
        /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and this <see cref="Backend.Recordsource.RecordSource"/>.
        /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
        /// </summary>
        public event FilterEventHandler? RunFilter;

        private bool VoidUpdate(CRUD? crud)
        {
            if (Controller is not IAbstractFormController formController) return false;

            if (crud != null)
                if (crud == CRUD.INSERT && formController.VoidParentUpdate) return true;
            else
                if (formController.VoidParentUpdate) return true;
            return false;
        }

        public override void Update(CRUD crud, ISQLModel model)
        {
            if (VoidUpdate(crud)) return;
            base.Update(crud, model);
            if (VoidUpdate(null)) return;
            RunFilter?.Invoke(this, new());
        }

        public new IEnumerator<M> GetEnumerator()
        {
            return (SourceNavigator<M>)GetSourceNavigator();
        }

        protected override INavigator GetSourceNavigator()
        {
            if (navigator != null)
            {
                navigator = new SourceNavigator<M>(this.Cast<M>(), navigator.Index);
                return (SourceNavigator<M>)navigator;
            }
            navigator = new SourceNavigator<M>(this.Cast<M>());
            return (SourceNavigator<M>)navigator;
        }

    }
}