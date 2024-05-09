using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FrontEnd.Forms.FormComponents
{
    /// <summary>
    /// This class represents a RecordTracker object. A RecordTracker object tells on which record the user is within the RecordSource object.
    /// This control is used within a <see cref="AbstractForm"/> object.
    /// </summary>
    public class RecordTracker : AbstractControl
    {
        static RecordTracker() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RecordTracker), new FrameworkPropertyMetadata(typeof(RecordTracker)));

        public RecordTracker() => OnClickCommand = new RecordTrackerClickCommand(OnClicked);

        #region OnClickCommand
        public ICommand OnClickCommand
        {
            get => (ICommand)GetValue(OnClickProperty);
            set => SetValue(OnClickProperty, value);
        }

        public static readonly DependencyProperty OnClickProperty =
            DependencyProperty.Register(nameof(OnClickCommand), typeof(ICommand), typeof(RecordTracker), new PropertyMetadata());
        #endregion

        #region Records
        public string Records
        {
            get => (string)GetValue(RecordsProperty);
            set => SetValue(RecordsProperty, value);
        }

        public static readonly DependencyProperty RecordsProperty =
            DependencyProperty.Register(nameof(Records), typeof(string), typeof(RecordTracker), new PropertyMetadata());
        #endregion

        #region GoNewVisibility
        public Visibility GoNewVisibility
        {
            get => (Visibility)GetValue(GoNewVisibilityProperty);
            set => SetValue(GoNewVisibilityProperty, value);
        }

        public static readonly DependencyProperty GoNewVisibilityProperty =
            DependencyProperty.Register(nameof(GoNewVisibility), typeof(Visibility), typeof(RecordTracker), new PropertyMetadata());
        #endregion

        protected override void OnControllerChanged(DependencyPropertyChangedEventArgs e)
        {
            DataContext = e.NewValue;
            Binding RecordDisplayerBinding = new("Records")
            {
                Source = DataContext,
            };

            SetBinding(RecordsProperty, RecordDisplayerBinding);

            Binding AllowNewRecordBinding = new("AllowNewRecord")
            {
                Source = DataContext,
                Converter = new AllowNewRecordConverter()
            };

            SetBinding(GoNewVisibilityProperty, AllowNewRecordBinding);
        }

        private void OnClicked(int movement)
        {
            switch (movement)
            {
                case 1:
                    Controller.GoFirst();
                    break;
                case 2:
                    Controller.GoPrevious();
                    break;
                case 3:
                    if (Controller.Source.Navigate().EOF) 
                    {
                        if (Controller.AllowNewRecord) 
                        {
                            if (IsListController) break;
                            else Controller.GoNew();
                        }
                    } 
                    else Controller.GoNext();
                    break;
                case 4:
                    Controller.GoLast();
                    break;
                case 5:
                    Controller.GoNew();
                    break;
            }
        }

        internal class RecordTrackerClickCommand(Action<int> _execute) : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private readonly Action<int> _execute = _execute;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                if (parameter == null) throw new Exception("Parameter was null");
                string? str = parameter.ToString();
                if (string.IsNullOrEmpty(str)) throw new Exception("Parameter was null");
                _execute(int.Parse(str));
            }
        }

        internal class AllowNewRecordConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool boolValue)
                {
                    return boolValue ? Visibility.Visible : Visibility.Hidden;
                }
                return Visibility.Visible;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is Visibility visibility)
                    return visibility.Equals(Visibility.Visible) ? true : false;
                return ">";
            }
        }
    }
}