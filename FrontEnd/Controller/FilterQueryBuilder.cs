using Backend.Database;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class helps build the Search Query and the parameters to be used when searching the recordset. 
    /// <para/>
    /// <c>IMPORTANT:</c>
    /// This class is not designed to handle Aggregation Queries.
    /// </summary>
    /// <param name="defaultQuery">The Default Query Structure; This will tipically be the <see cref="AbstractListController{M}.DefaultSearchQry"/> property.</param>
    public class FilterQueryBuilder(string defaultQuery)
    {
        private readonly string _defaultQuery = defaultQuery;
        private string _qry = string.Empty;
        private readonly List<QueryParameter> _parameters = [];

        /// <summary>
        /// Gets the List of parameters to be used in the Search Query.
        /// </summary>
        public List<QueryParameter> Params => _parameters;

        /// <summary>
        /// The final query to be used in the Search.
        /// </summary>
        public string Query { get => string.IsNullOrEmpty(_qry) ? _defaultQuery : _qry; }

        /// <summary>
        /// Reset the object to its initial state.
        /// </summary>
        public void Clear()
        {
            _qry = string.Empty;
            _parameters.Clear();
        }

        /// <summary>
        /// Add a parameter to the <see cref="Params"/> property.
        /// </summary>
        /// <param name="placeholder">A string representing the placeholder. e.g. @name</param>
        /// <param name="value">The value of the parameter</param>
        public void AddParameter(string placeholder, object? value) => _parameters.Add(new(placeholder, value));

        /// <summary>
        /// Add a condition to the WHERE clause to the Default Query. <para/>
        /// </summary>
        /// <param name="condition"></param>
        public void AddCondition(string condition)
        {
            if (condition.Length > 0)
            {
                if (string.IsNullOrEmpty(_qry))
                    _qry = _defaultQuery;

                _qry += $" AND ({condition})";
            }
        }
    }
}
