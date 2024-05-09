using FrontEnd.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FrontEnd.Reports
{
    public class ReportPage : Control, IReportPage
    {
        static ReportPage() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportPage), new FrameworkPropertyMetadata(typeof(ReportPage)));
        private Grid? grid;
        public ReportPage()
        {
            AdjustPageSize();
            Background = Brushes.White;
            LayoutUpdated += ReportPage_LayoutUpdated;
        }

        private void ReportPage_LayoutUpdated(object? sender, EventArgs e)
        {
            HeaderHeight = grid?.RowDefinitions[0]?.ActualHeight;
            MainHeight = grid?.RowDefinitions[1]?.ActualHeight;
            FooterHeight = grid?.RowDefinitions[2]?.ActualHeight;
            Total = HeaderHeight + MainHeight + FooterHeight;
            ContentOverflown = Total > PageHeight;
        }

        public ReportPage Copy() 
        {
            ReportPage page = new();
            page.FooterRow = this.FooterRow;
            page.HeaderRow = this.HeaderRow;
            page.Header = this.Header;
            page.Main = this.Main;
            page.Footer = this.Footer;
            page.Padding = this.Padding;
            page.PageNumber = this.PageNumber;
            return page;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            grid = (Grid?)GetTemplateChild("Page");
        }
        
        private double? Total { get; set; }
        private double? MainHeight { get; set; }
        private double? HeaderHeight { get; set; }
        private double? FooterHeight { get; set; }
        public bool ContentOverflown { get; private set; }

        #region PageWidth
        public static readonly DependencyProperty PageWidthProperty =
         DependencyProperty.Register(nameof(PageWidth), typeof(double), typeof(ReportPage), new PropertyMetadata());
        public double PageWidth 
        { 
            get => (double)GetValue(PageWidthProperty);
            set => SetValue(PageWidthProperty, value);
        }
        #endregion

        #region PageHeight
        public static readonly DependencyProperty PageHeightProperty =
         DependencyProperty.Register(nameof(PageHeight), typeof(double), typeof(ReportPage), new PropertyMetadata());
        public double PageHeight 
        { 
            get => (double)GetValue(PageHeightProperty); 
            set => SetValue(PageHeightProperty, value); 
        }
        #endregion

        #region Header
        /// <summary>
        /// Gets and Sets the Form Header.
        /// </summary>
        public UIElement Header
        {
            get => (UIElement)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(UIElement), typeof(ReportPage), new PropertyMetadata());
        #endregion

        #region Footer
        /// <summary>
        /// Gets and Sets the Form Header.
        /// </summary>
        public UIElement Footer
        {
            get => (UIElement)GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register(nameof(Footer), typeof(UIElement), typeof(ReportPage), new PropertyMetadata());
        #endregion

        #region Main
        /// <summary>
        /// Gets and Sets the Form Header.
        /// </summary>
        public UIElement Main
        {
            get => (UIElement)GetValue(MainProperty);
            set => SetValue(MainProperty, value);
        }

        public static readonly DependencyProperty MainProperty =
            DependencyProperty.Register(nameof(Main), typeof(UIElement), typeof(ReportPage), new PropertyMetadata());
        #endregion

        #region HeaderRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Header"/> property.
        /// </summary>
        public GridLength HeaderRow
        {
            get => (GridLength)GetValue(HeaderRowProperty);
            set => SetValue(HeaderRowProperty, value);
        }

        public static readonly DependencyProperty HeaderRowProperty =
            DependencyProperty.Register(nameof(HeaderRow), typeof(GridLength), typeof(ReportPage), new PropertyMetadata(new GridLength(30), null));
        #endregion

        #region FooterRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Header"/> property.
        /// </summary>
        public GridLength FooterRow
        {
            get => (GridLength)GetValue(FooterRowProperty);
            set => SetValue(FooterRowProperty, value);
        }

        public static readonly DependencyProperty FooterRowProperty =
            DependencyProperty.Register(nameof(FooterRow), typeof(GridLength), typeof(ReportPage), new PropertyMetadata(new GridLength(30), null));
        #endregion

        #region PageNumber
        public static readonly DependencyProperty PageNumberProperty =
         DependencyProperty.Register(nameof(PageNumber), typeof(int), typeof(ReportPage), new PropertyMetadata(1));

        public int PageNumber 
        { 
            get => (int) GetValue(PageNumberProperty);
            set => SetValue(PageNumberProperty, value);
        }
        #endregion
        private void AdjustPageSize()
        {
            var dpiInfo = VisualTreeHelper.GetDpi(this); // Get DPI information

            double dpiX = dpiInfo.DpiScaleX;
            double dpiY = dpiInfo.DpiScaleY;

            PageWidth = (210 / 25.4) * 96 * dpiX;  // Convert mm to inches, then to device-independent pixels
            PageHeight = (297 / 25.4) * 96 * dpiY; // Convert mm to inches, then to device-independent pixels
        }
    }
}