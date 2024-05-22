namespace Backend.Events
{

    public delegate void InternetConnectionStatusHandler(object? sender, InternetConnectionStatusArgs e);

    /// <summary>
    /// This delegate works as a bridge between the <see cref="FrontEnd.Controller.AbstractFormListController{M}"/> and a <see cref="FrontEnd.Source.RecordSource{M}"/>.
    /// <para/>
    /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
    /// <para/>
    /// This delegate is called in the <see cref="Backend.Source.IChildSource.Update(Backend.Database.CRUD, Backend.Model.ISQLModel)"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FilterEventHandler(object? sender, EventArgs e);

    public class InternetConnectionStatusArgs : EventArgs
    { 
        public bool IsConnected { get; }
        public string Message => IsConnected ? "" : "No Connection";

        public InternetConnectionStatusArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}
