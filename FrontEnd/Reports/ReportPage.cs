using FrontEnd.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrontEnd.Reports
{
    /// <summary>
    /// It instantiates a ReportPage object which can be viewed by the <see cref="ReportViewer"/>
    /// </summary>
    public class ReportPage : Control, IReportPage
    {
        private Grid? grid;
        private double? Total { get; set; }
        private double? MainHeight { get; set; }
        private double? HeaderHeight { get; set; }
        private double? FooterHeight { get; set; }
        public bool ContentOverflown { get; private set; }

        public static readonly DependencyProperty PaddingPageProperty = DependencyProperty.Register(nameof(PaddingPage), typeof(Thickness), typeof(ReportPage), new PropertyMetadata(OnPaddingPagePropertyPage));

        public static readonly DependencyProperty PaddingHeaderBodyProperty = DependencyProperty.Register(nameof(PaddingHeaderBody), typeof(Thickness), typeof(ReportPage), new PropertyMetadata());

        public static readonly DependencyProperty PaddingFooterProperty = DependencyProperty.Register(nameof(PaddingFooter), typeof(Thickness), typeof(ReportPage), new PropertyMetadata());
        public Thickness PaddingHeaderBody
        {
            get => (Thickness)GetValue(PaddingHeaderBodyProperty);
            private set => SetValue(PaddingHeaderBodyProperty, value);
        }

        public Thickness PaddingFooter
        {
            get => (Thickness)GetValue(PaddingFooterProperty);
            private set => SetValue(PaddingFooterProperty, value);
        }
        public Thickness PaddingPage 
        { 
            get => (Thickness)GetValue(PaddingPageProperty); 
            set 
            {
                SetValue(PaddingPageProperty, value);
            } 
        }
        private static void OnPaddingPagePropertyPage(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReportPage page = (ReportPage)d;
            Thickness value = (Thickness)e.NewValue;
            page.PaddingHeaderBody = new(value.Left, value.Top, value.Right, 0);
            page.PaddingFooter = new(value.Left, 0, value.Right, value.Bottom);
        }

        public ReportPage()
        {
            AdjustPageSize();
            Background = Brushes.White;
            LayoutUpdated += OnLayoutUpdated;
        }
        static ReportPage() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportPage), new FrameworkPropertyMetadata(typeof(ReportPage)));

        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            HeaderHeight = grid?.RowDefinitions[0]?.ActualHeight;
            MainHeight = grid?.RowDefinitions[1]?.ActualHeight;
            FooterHeight = grid?.RowDefinitions[2]?.ActualHeight;
            Total = HeaderHeight + MainHeight + FooterHeight;
            ContentOverflown = Total > PageHeight;
        }

        /// <summary>
        /// Copy the content from another ReportPage.
        /// </summary>
        /// <param name="page">Another Report Page</param>
        public void CopyFrom(ReportPage page)
        {
            PaddingPage = page.PaddingPage;
            PageNumber = page.PageNumber;
            HeaderRow = page.HeaderRow;
            FooterRow = page.FooterRow;
            Header = page.Header;
            Body = page.Body;
            Footer = page.Footer;
        }

        /// <summary>
        /// Creates a new ReportPage with the same content as this one.
        /// </summary>
        /// <returns>A new ReportPage</returns>
        public ReportPage Copy() 
        {
            ReportPage page = new();
            page.PageNumber = PageNumber;
            page.PaddingPage = PaddingPage;
            page.HeaderRow = HeaderRow;
            page.FooterRow = FooterRow;
            page.Header = Header;
            page.Body = Body;
            page.Footer = Footer;
            return page;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            grid = (Grid?)GetTemplateChild("Page");
        }
        
        #region PageWidth
        public static readonly DependencyProperty PageWidthProperty =
         DependencyProperty.Register(nameof(PageWidth), typeof(double), typeof(ReportPage), new PropertyMetadata());

        /// <summary>
        /// Gets the Width of the Page Report. 
        /// This property is set in the Constructor where the <see cref="AdjustPageSize"/>
        /// ensures the Width is set to represents an A4's width.
        /// </summary>
        public double PageWidth 
        { 
            get => (double)GetValue(PageWidthProperty);
            private set => SetValue(PageWidthProperty, value);
        }
        #endregion

        #region PageHeight
        public static readonly DependencyProperty PageHeightProperty =
         DependencyProperty.Register(nameof(PageHeight), typeof(double), typeof(ReportPage), new PropertyMetadata());
        /// <summary>
        /// Gets the Height of the Page Report. 
        /// This property is set in the Constructor where the <see cref="AdjustPageSize"/>
        /// ensures the Height is set to represents an A4's height.
        /// </summary>
        public double PageHeight 
        { 
            get => (double)GetValue(PageHeightProperty); 
            private set => SetValue(PageHeightProperty, value); 
        }
        #endregion

        #region Header
        /// <summary>
        /// Gets and Sets the Page Header.
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
        /// Gets and Sets the Page's Footer.
        /// </summary>
        public UIElement Footer
        {
            get => (UIElement)GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register(nameof(Footer), typeof(UIElement), typeof(ReportPage), new PropertyMetadata());
        #endregion

        #region Body
        /// <summary>
        /// Gets and Sets the Page's body.
        /// </summary>
        public UIElement Body
        {
            get => (UIElement)GetValue(BodyProperty);
            set => SetValue(BodyProperty, value);
        }

        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register(nameof(Body), typeof(UIElement), typeof(ReportPage), new PropertyMetadata());
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

        /// <summary>
        /// Gets and Sets the Page's Number.
        /// </summary>
        public int PageNumber 
        { 
            get => (int) GetValue(PageNumberProperty);
            set => SetValue(PageNumberProperty, value);
        }
        #endregion
        
        /// <summary>
        /// Ensure the Page' sizes are set to A4 based on screen's DIP
        /// </summary>
        private void AdjustPageSize()
        {
            DpiScale dpiInfo = VisualTreeHelper.GetDpi(this);

            double dpiX = dpiInfo.DpiScaleX;
            double dpiY = dpiInfo.DpiScaleY;

            PageWidth = (210 / 25.4) * 96 * dpiX;  // Convert mm to inches, then to device-independent pixels
            PageHeight = (297 / 25.4) * 96 * dpiY; // Convert mm to inches, then to device-independent pixels
        }

    }
}