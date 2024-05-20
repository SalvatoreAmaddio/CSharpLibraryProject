using Backend.Model;
using Backend.Source;
using FrontEnd.Controller;
using FrontEnd.Events;
using System.ComponentModel;
using System.Text;
using Backend.Database;
using FrontEnd.Forms;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// This interface extends <see cref="INotifyPropertyChanged"/> and defines the properties and methods to be implemented by the <see cref="FilterOption"/> class.
    /// <para/>
    /// This interface works in conjunction with the <see cref="HeaderFilter"/> GUI Control.
    /// </summary>
    public interface IFilterOption : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets and sets a boolean indicating if an option has been selected. This property triggers the <see cref="OnSelectionChanged"/> event.
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
        /// Occurs when an option is selected or deselected.
        /// </summary>
        public event SelectionChangedEventHandler? OnSelectionChanged;
        
        /// <summary>
        /// Deselects an option bypassing the <see cref="OnSelectionChanged"/> event.
        /// </summary>
        public void Deselect();
    }

    /// <summary>
    /// Concrete impementation of the <see cref="IFilterOption"/>
    /// <para/>
    /// This class works in conjunction with the <see cref="Forms.HeaderFilter"/> GUI Control.
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

        public override bool Equals(object? obj) =>
        obj is FilterOption option && Record.Equals(option.Record);

        public override int GetHashCode() => HashCode.Combine(Record);
    }

    /// <summary>
    /// It instantiates a List&lt;<see cref="IFilterOption"/>> object.
    /// <para/>
    /// This class works in conjunction with the <see cref="HeaderFilter"/> class.
    /// </summary>
    /// <param name="source">A RecordSource object</param>
    /// <param name="displayProperty">The Record's property to display in the option list.</param>
    public class SourceOption : List<IFilterOption>, IChildSource
    {
        private readonly string _displayProperty;
        /// <summary>
        /// A list of <see cref="IUIControl"/> associated to this <see cref="RecordSource"/>.
        /// </summary>
        private List<IUIControl>? UIControls;

        public IParentSource? ParentSource { get; set; }

        public SourceOption(RecordSource source, string displayProperty) : base(source.Select(s=>new FilterOption(s,displayProperty)))
        {
            _displayProperty = displayProperty;
            source.ParentSource?.AddChild(this);
        }

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

        /// <summary>
        /// It adds a <see cref="IUIControl"/> object to the <see cref="UIControls"/>.
        /// <para/>
        /// If <see cref="UIControls"/> is null, it gets initialised.
        /// </summary>
        /// <param name="control">An object implementing <see cref="IUIControl"/></param>
        public void AddUIControlReference(IUIControl control)
        {
            if (UIControls == null) UIControls = [];
            UIControls.Add(control);
        }

        /// <summary>
        /// This method is called in <see cref="Update(CRUD, ISQLModel)"/>.
        /// It loops through the <see cref="UIControls"/> to notify the <see cref="IUIControl"/> object to reflect changes that occured to their ItemsSource which is an instance of <see cref="RecordSource"/>.
        /// </summary>
        private void NotifyUIControl()
        {
            if (UIControls != null && UIControls.Count > 0)
                foreach (IUIControl combo in UIControls) combo.OnItemSourceUpdated();
        }

        public void Update(CRUD crud, ISQLModel model)
        {
            FilterOption option = new(model, _displayProperty);
            switch (crud)
            {
                case CRUD.INSERT:
                    Add(option);
                    break;
                case CRUD.UPDATE:
                    int index = IndexOf(option);
                    IFilterOption oldValue = this[index];
                    bool isSelected = oldValue.IsSelected;
                    this[index] = option;
                    this[index].IsSelected = isSelected;
                    NotifyUIControl();
                    break;
                case CRUD.DELETE:
                     Remove(option);
                    break;
            }
        }
    }
}