using Backend.Model;

namespace Backend.Source
{

    /// <summary>
    /// This interface extends the IEnumerator&lt;ISQLModel&gt; 
    /// to add extra functionalities.
    /// For instance, this enumerator can move up and down the IEnumerable.
    /// This interface is meant for dealing with IEnumerable&lt;<see cref="ISQLModel"/>&gt; only.
    /// </summary>
    public interface ISourceNavigator : IEnumerator<ISQLModel?>, INavigator { }
}
