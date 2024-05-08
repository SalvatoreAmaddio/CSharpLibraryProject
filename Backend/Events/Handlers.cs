
namespace Backend.Events
{
    /// <summary>
    /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and a <see cref="Recordsource.RecordSource"/>.
    /// <para/>
    /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
    /// <para/>
    /// This delegate is called on the <see cref="Recordsource.IChildSource.Update(Database.CRUD, Model.ISQLModel)"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FilterEventHandler(object? sender, EventArgs e);

}
