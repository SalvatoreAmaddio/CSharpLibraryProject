using Backend.Source;
using FrontEnd.Model;

namespace FrontEnd.Source
{
    public interface ISourceNavigator<M> : IEnumerator<M?>, INavigator where M : AbstractModel, new()
    {

    }

}
