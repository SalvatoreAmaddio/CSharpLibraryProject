using Backend.Model;
using Backend.Recordsource;
using FrontEnd.Controller;
using FrontEnd.Events;
using System.ComponentModel;
using System.Text;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// This interface extends <see cref="INotifyPropertyChanged"/> and defines the properties and methods to be implemented by the <see cref="FilterOption"/> class.
    /// </summary>
    public interface IFilterOption : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets and sets a boolean indicating if an option has been selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets the ISQLModel that can be selected as option.
        /// </summary>
        public ISQLModel Record { get; }

        /// <summary>
        /// Gets the value of the <see cref="Record"/> property to be displayed.
        /// </summary>
        public object? Value { get; }        
        
        /// <summary>
        /// Event that triggers when an option is selected or deselected.
        /// </summary>
        public event SelectionChangedEventHandler? OnSelectionChanged;
        
        /// <summary>
        /// Deselects an option bypassing the <see cref="OnSelectionChanged"/> event.
        /// </summary>
        public void Deselect();
    }

    /// <summary>
    /// Concrete impementation of the <see cref="IFilterOption"/>
    /// </summary>
    public class FilterOption : IFilterOption
    {
        private bool _isSelected = false;
        public object? Value { get; }
        public ISQLModel Record { get; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                OnSelectionChanged?.Invoke(this, new EventArgs());
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event SelectionChangedEventHandler? OnSelectionChanged;

        public FilterOption(ISQLModel record, string displayProperty) 
        {
            Record = record;
            ITableField Field = Record.GetTableFields().First(s => s.Name.Equals(displayProperty));
            Value = Field.GetValue();
        }

        public void Deselect()
        {
            _isSelected = false;
            PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
        }

    }

    /// <summary>
    /// A List for dealing with <see cref="IFilterOption"/> objcets.
    /// </summary>
    /// <param name="source">A RecordSource object</param>
    /// <param name="displayProperty">The Record's property to display in the option list.</param>
    public class SourceOption(RecordSource source, string displayProperty) : List<IFilterOption>(source.Select(s => new FilterOption(s, displayProperty)))
    {
        /// <summary>
        /// It loops through the List and builds the SQL logic to filter the Select the statement.
        /// </summary>
        /// <param name="filterQueryBuilder"></param>
        /// <returns>A string</returns>
        public string Conditions(FilterQueryBuilder filterQueryBuilder) 
        {
            StringBuilder sb = new();
            int i = 0;

            foreach(var item in this) 
            {
                if (item.IsSelected) 
                {
                    string? fieldName = item?.Record?.GetTablePK()?.Name;
                    sb.Append($"{fieldName} = @{fieldName}{++i} OR ");
                    filterQueryBuilder.AddParameter($"{fieldName}{i}", item?.Record?.GetTablePK()?.GetValue());
                }
            }
            
            if (sb.Length > 0) 
            {
                sb.Remove(sb.Length - 1, 1);
                if (sb.ToString(sb.Length - 2, 2) == "OR") 
                {
                    sb.Remove(sb.Length - 2, 2);
                    sb.Remove(sb.Length - 1, 1);
                }
            }

            return sb.ToString();
        }
    }
}
