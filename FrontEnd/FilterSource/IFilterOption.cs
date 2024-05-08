using Backend.Model;
using Backend.Recordsource;
using FrontEnd.Controller;
using System.ComponentModel;
using System.Text;

namespace FrontEnd.FilterSource
{
    public delegate void OnSelected(object? sender, OnSelectedEventArgs e);

    public class OnSelectedEventArgs(bool selected, ISQLModel? record) : EventArgs 
    {
        public bool IsSelected { get; } = selected;
        public ISQLModel? Record { get; } = record;
    }
    public interface IFilterOption : INotifyPropertyChanged
    {
        public bool IsSelected { get; set; }
        public ISQLModel Record { get; }
        public object? Value { get; }        
        public event OnSelected? OnSelected;
        public void Unset();
    }

    public class FilterOption : IFilterOption
    {
        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                OnSelected?.Invoke(this, new OnSelectedEventArgs(value, Record));
            }
        }

        public void Unset() 
        {
            _isSelected = false;
            PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
        }

        public object? Value { get; }
        public ISQLModel Record { get; }

        public FilterOption(ISQLModel record, string displayProperty) 
        {
            Record = record;
            ITableField Field = Record.GetTableFields().First(s => s.Name.Equals(displayProperty));
            Value = Field.GetValue();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event OnSelected? OnSelected;

    }

    public class SourceOption(RecordSource source, string displayProperty) : List<IFilterOption>(source.Select(s => new FilterOption(s, displayProperty)))
    {
        public string Conditions(FilterQueryBuilder filterQueryBuilder) 
        {
            StringBuilder sb = new();

            foreach(var item in this) 
            {
                if (item.IsSelected) 
                {
                    string? fieldName = item?.Record?.GetTablePK()?.Name;
                    sb.Append($"{fieldName} = @{fieldName} OR ");
                    filterQueryBuilder.AddParameter($"{fieldName}", item?.Record?.GetTablePK()?.GetValue());
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
