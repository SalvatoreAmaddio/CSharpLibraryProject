using Backend.Utils;
using FrontEnd.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontEnd.Forms
{
    public class Curtain : ContentControl
    {
        Button? PART_CloseButton;
        Hyperlink? PART_WebLink;
        static Curtain()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Curtain), new FrameworkPropertyMetadata(typeof(Curtain)));
        }

        #region SoftwareInfo
        public SoftwareInfo SoftwareInfo
        {
            get => (SoftwareInfo)GetValue(SoftwareInfoProperty);
            set => SetValue(SoftwareInfoProperty, value);
        }

        public static readonly DependencyProperty SoftwareInfoProperty = DependencyProperty.Register(nameof(SoftwareInfo), typeof(SoftwareInfo), typeof(Curtain), new PropertyMetadata(OnSoftwareInfoPropertyChanged));

        private static void OnSoftwareInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Curtain curtain = (Curtain)d;
            curtain.SetBinding(DeveloperNameProperty, new Binding(nameof(DeveloperName)) { Source = e.NewValue });
            curtain.SetBinding(SoftwareYearProperty, new Binding(nameof(SoftwareYear)) { Source = e.NewValue });
            curtain.SetBinding(DeveloperWebsiteProperty, new Binding(nameof(DeveloperWebsite)) { Source = e.NewValue });
            curtain.SetBinding(SoftwareNameProperty, new Binding(nameof(SoftwareName)) { Source = e.NewValue });
            curtain.SetBinding(SoftwareVersionProperty, new Binding(nameof(SoftwareVersion)) { Source = e.NewValue });
        }

        #endregion

        #region DeveloperName
        public string DeveloperName
        {
            get => (string)GetValue(DeveloperNameProperty);
            set => SetValue(DeveloperNameProperty, value);
        }

        public static readonly DependencyProperty DeveloperNameProperty = DependencyProperty.Register(nameof(DeveloperName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareYear
        public string SoftwareYear
        {
            get => (string)GetValue(SoftwareYearProperty);
            set => SetValue(SoftwareYearProperty, value);
        }

        public static readonly DependencyProperty SoftwareYearProperty = DependencyProperty.Register(nameof(SoftwareYear), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region DeveloperWebsite
        public Uri DeveloperWebsite
        {
            get => (Uri)GetValue(DeveloperWebsiteProperty);
            set => SetValue(DeveloperWebsiteProperty, value);
        }

        public static readonly DependencyProperty DeveloperWebsiteProperty = DependencyProperty.Register(nameof(DeveloperWebsite), typeof(Uri), typeof(Curtain), new PropertyMetadata());
        #endregion

        #region HeaderTitle
        public string HeaderTitle
        {
            get => (string)GetValue(HeaderTitleProperty);
            set => SetValue(HeaderTitleProperty, value);
        }

        public static readonly DependencyProperty HeaderTitleProperty = DependencyProperty.Register(nameof(HeaderTitle), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareName
        public string SoftwareName
        {
            get => (string)GetValue(SoftwareNameProperty);
            set => SetValue(SoftwareNameProperty, value);
        }

        public static readonly DependencyProperty SoftwareNameProperty = DependencyProperty.Register(nameof(SoftwareName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareVersion
        public string SoftwareVersion
        {
            get => (string)GetValue(SoftwareVersionProperty);
            set => SetValue(SoftwareVersionProperty, value);
        }

        public static readonly DependencyProperty SoftwareVersionProperty = DependencyProperty.Register(nameof(SoftwareVersion), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_CloseButton = (Button?)GetTemplateChild(nameof(PART_CloseButton));
            if (PART_CloseButton == null) throw new Exception($"Failed to get {nameof(PART_CloseButton)}");
            PART_CloseButton.Click += OnCloseButtonClicked;

            PART_WebLink = (Hyperlink?)GetTemplateChild(nameof(PART_WebLink));
            if (PART_WebLink == null) throw new Exception($"Failed to get {nameof(PART_WebLink)}");

            PART_WebLink.RequestNavigate += OnHyperlinkClicked;
            string url = DeveloperWebsite.AbsoluteUri;

            try 
            {
                url = url.Split("//")[1];
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);
            }
            catch 
            { 
                            
            }
            PART_WebLink.Inlines.Add(url);
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void OnHyperlinkClicked(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo info = new(e.Uri.AbsoluteUri);
            info.UseShellExecute = true;
            Process.Start(info);
            e.Handled = true;
        }
    }
}