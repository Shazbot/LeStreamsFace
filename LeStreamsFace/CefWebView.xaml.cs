﻿using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CefSharp;
using System;
using System.Windows;
using System.Windows.Controls;
using CefSharp.Wpf;

namespace LeStreamsFace
{
    /// <summary>
    /// Interaction logic for CefWebView.xaml
    /// </summary>
    public partial class CefWebView : UserControl, INotifyPropertyChanged
    {
        public CefWebView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void WebBrowserOnLoadCompleted(object sender, LoadCompletedEventArgs url)
        {
            Debugger.Break();
        }

        private IWpfWebBrowser webBrowser;

        public IWpfWebBrowser WebBrowser
        {
            get { return webBrowser; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref webBrowser, value, () => WebBrowser);
            }
        }
        public static void Init()
        {
            var settings = new CefSettings()
                           {
//                               BrowserSubprocessPath = "CefSharp.BrowserSubprocess.exe"
                           };
//            settings.RegisterScheme(new CefCustomScheme
//            {
//                SchemeName = CefSharpSchemeHandlerFactory.SchemeName,
//                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory()
//            });
            Cef.Initialize(settings);

            //            CefSharp.Settings settings = new CefSharp.Settings();
            //            if (CEF.Initialize(settings))
            //            {
            //                CEF.RegisterScheme("theme", new ThemeSchemeHandlerFactory());
            //                //CEF.RegisterScheme("test", new SchemeHandlerFactory());
            //                //CEF.RegisterJsObject("bound", new BoundObject());
            //            }
        }

        public static string BaseDirectory { get; set; }

        #region public string Html

        public static DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(CefWebView), new PropertyMetadata(" ", HtmlChanged));

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        #endregion public string Html

        #region public string FileName

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(CefWebView), new PropertyMetadata("", FileNameChanged));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        #endregion public string FileName

        #region public double ScrollPercentage

        public static DependencyProperty ScrollPercentageProperty =
            DependencyProperty.Register("ScrollPercentage", typeof(double), typeof(CefWebView), new PropertyMetadata(0d, ScrollPercentageChanged));

        public string ScrollPercentage
        {
            get { return (string)GetValue(ScrollPercentageProperty); }
            set { SetValue(ScrollPercentageProperty, value); }
        }

        #endregion public double ScrollPercentage

        public void Print()
        {
//            if (webView.IsBrowserInitialized)
//                webView.Print();
        }

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // does this even fire
            // IsBrowserInitialized should maybe be webView.WebBrowser != null
            var htmlPreview = (CefWebView)d;
            if (htmlPreview.webView != null && htmlPreview.webView.IsBrowserInitialized)
                htmlPreview.webView.LoadHtml((string)e.NewValue, "url");
        }

        private static void FileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var htmlPreview = (CefWebView)d;
            if (htmlPreview.webView != null && htmlPreview.webView.IsBrowserInitialized)
                htmlPreview.webView.Title = (string)e.NewValue;
        }

        private static void ScrollPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var htmlPreview = (CefWebView)d;
            if (htmlPreview.webView != null && htmlPreview.webView.IsBrowserInitialized)
            {
                var javascript = string.Format("window.scrollTo(0,{0} * (document.body.scrollHeight - document.body.clientHeight));", e.NewValue);
                htmlPreview.webView.EvaluateScript(javascript);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

//            webView.PropertyChanged += webView_PropertyChanged;
        }

        private void webView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (webView == null) return;

            switch (e.PropertyName)
            {
                case "IsBrowserInitialized":
                    if (webView.IsBrowserInitialized)
                        Dispatcher.BeginInvoke((Action)InitializeData);
                    break;
            }
        }

        private void InitializeData()
        {
            webView.LoadHtml(Html, "www.google.com");
            webView.Title = FileName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void WebView_OnLoaded(object sender, RoutedEventArgs e)
        {     
//            return;
            var wings = @"<object type=""application/x-shockwave-flash"" height=""" + "100%"+ @""" width=""" + "100%" + @""" id=""live_embed_player_flash"" data=""http://www.twitch.tv/widgets/live_embed_player.swf?channel=wingsofdeath"" bgcolor=""#000000""><param name=""allowFullScreen"" value=""false"" /><param name=""allowScriptAccess"" value=""always"" /><param name=""allowNetworking"" value=""all"" /><param name=""movie"" value=""http://www.twitch.tv/widgets/live_embed_player.swf"" /><param name=""flashvars"" value=""hostname=www.twitch.tv&channel=wingsofdeath&auto_play=true&start_volume=25"" /></object>";
            Console.WriteLine(wings);
            //webView.LoadHtml(wings, "arst");// = wings;
        }

        private void WebView_OnToolTipOpening(object sender, ToolTipEventArgs e)
        {
            e.Handled = true;
        }

        private void WebView_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}