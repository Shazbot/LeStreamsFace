﻿using Caliburn.Micro;
using CefSharp;
using CefSharp.Wpf;
using LeStreamsFace.Annotations;
using OxyPlot.Reporting;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Action = System.Action;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace LeStreamsFace
{
    [ImplementPropertyChanged]
    public partial class CefWebView : UserControl, INotifyPropertyChanged
    {
        // we need to manually dispose of webViews
        public static readonly Dictionary<Stream, CefWebView> WebViewForStream = new Dictionary<Stream, CefWebView>();

        public CefWebView()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static void Init()
        {
            CefSharp.Settings settings = new CefSharp.Settings();
            if (CEF.Initialize(settings))
            {
                //                CEF.RegisterScheme("theme", new ThemeSchemeHandlerFactory());
                //CEF.RegisterScheme("test", new SchemeHandlerFactory());
                //CEF.RegisterJsObject("bound", new BoundObject());
            }
        }

        private void BrowserOnIsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if ((bool)dependencyPropertyChangedEventArgs.NewValue)
            {
                Dispatcher.BeginInvoke((Action)InitializeData);
            }
        }

        public string Address { get; set; }

        public string Title { get; set; }

        #region public string Html

        public static DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(CefWebView), new PropertyMetadata(" ", HtmlChanged));

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set
            {
                SetValue(HtmlProperty, value);
            }
        }

        #endregion public string Html

        private static List<CefWebView> views = new List<CefWebView>();

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var host = (CefWebView)d;
            if (!views.Contains(host)) views.Add(host); // check if we are reusing views in tabcontrol
            if (host.browser == null) return;

            var html = ((string)(e.NewValue));
            //            host.browser.LoadHtml(html, "url");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            browser.PropertyChanged += host_PropertyChanged;
        }

        private void host_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBrowserInitialized":
                    if (browser.IsBrowserInitialized)
                        Dispatcher.BeginInvoke((Action)InitializeData);
                    break;
            }
        }

        private void InitializeData()
        {
            WebViewForStream[((Stream)Tag)] = this;
            browser.LoadHtml(Html, "url");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            //                        Console.WriteLine("x is " + e.GetPosition(browser).X);
            //                        Console.WriteLine("y is " + e.GetPosition(browser).Y);
            //                        Console.WriteLine("b width is " + browser.Width + "       " + browser.ActualWidth);
            //                        Console.WriteLine("b height is " + browser.Height + "       " + browser.ActualHeight);
        }

        private bool waitForQualityOptionsClick;

        private void Browser_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (waitForQualityOptionsClick)
            {
                waitForQualityOptionsClick = false;
                Overlay.Visibility = Visibility.Visible;
                e.Handled = true;
                return;
            }

            var x = e.GetPosition(this).X;
            if (x >= browser.ActualWidth - 27) // fake maximize
            {
                EventAggregatorExtensions.PublishOnUIThread(MainWindow.EventAggregator, new MinimizeMaximizeMessage());
                e.Handled = true;
            }
            else if (x >= browser.ActualWidth - 110 && x <= browser.ActualWidth - 58)
            {
                waitForQualityOptionsClick = true;
                Overlay.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }

            //            if (StreamsListWindow.IsMaximized)
            //            {
            //                return;
            //            }
            //            var x = e.GetPosition(this).X;
            //            if (x >= 4 && x <= 25) // pause by sending space
            //            {
            //                SendKeys.SendWait(" ");
            //            }
            //            else if (x >= 30 && x <= 55) // mute by sending a click at the low end of the volume slider
            //            {
            //                var muteClickPoint = PointToScreen(new Point(60, browser.ActualHeight - 10));
            //                NativeMethods.LeftClick((int)muteClickPoint.X, (int)muteClickPoint.Y);
            //            }

            //            e.Handled = true;
            Console.WriteLine("x is " + e.GetPosition(browser).X);
            Console.WriteLine("y is " + e.GetPosition(browser).Y);
            Console.WriteLine("b width is " + browser.Width + "       " + browser.ActualWidth);
            Console.WriteLine("b height is " + browser.Height + "       " + browser.ActualHeight);
        }

        private void Browser_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //                        var x = e.GetPosition(this).X;
            //                        if (x >= browser.ActualWidth - 27) // ignore maximize
            //                        {
            //                            e.Handled = true;
            //                        }
        }

        private void Browser_OnMouseMove(object sender, MouseEventArgs e)
        {
            //            Console.WriteLine("x is " + e.GetPosition(browser).X);
            //            Console.WriteLine("y is " + e.GetPosition(browser).Y);
            //            Console.WriteLine("b width is " + browser.Width + "       " + browser.ActualWidth);
            //            Console.WriteLine("b height is " + browser.Height + "       " + browser.ActualHeight);
            //            Console.WriteLine("".PadRight(20, '-'));
        }

        private void Browser_OnUnloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}