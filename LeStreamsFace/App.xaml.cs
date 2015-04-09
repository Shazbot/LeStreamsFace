﻿using LeStreamsFace.StreamParsers;
using Mindscape.Raygun4Net;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LeStreamsFace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex;

        public static readonly int WM_SHOWFIRSTINSTANCE = NativeMethods.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|" + Assembly.GetEntryAssembly().GetName().Name + "|" + ProgramInfo.AssemblyGuid);

        private RaygunClient _raygunClient = new RaygunClient("Fyi8SY7+vLuMQWj+FqLx5g==");

        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            File.WriteAllText("crash " + DateTime.Now.ToString().Replace(':', '-') + ".txt", e.Exception.ToString());
            _raygunClient.Send(e.Exception);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool onlyInstance = false;
            mutex = new Mutex(true, "LeStreamsFaceMutex", out onlyInstance);
            if (!onlyInstance)
            {
                NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
                //                MessageBox.Show("Another instance of the application already running.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ExitApp();
                return;
            }

            base.OnStartup(e);

            // now using Costura to load assemblies
            //            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Target);

            new AppLogic(new TwitchXMLStreamParser(), new TwitchJSONStreamParser());

            CefWebView.Init();
        }

        internal static void ExitApp()
        {
            if (Application.Current == null) return;

            Application.Current.Shutdown();
        }

        private Assembly Target(object sender, ResolveEventArgs args)
        {
            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            string[] fields = args.Name.Split(',');
            string name = fields[0];
            string culture = fields[2];

            //A satellite assembly ends with .resources and uses a specific culture
            if (name.EndsWith(".resources") && !culture.EndsWith("neutral")) return null;

            // loading DllName.dll from resource AssemblyName.lib.DllName.dll
            String resourceName = Assembly.GetEntryAssembly().GetName().Name + ".lib." + new AssemblyName(args.Name).Name + ".dll";
            if (resourceNames.All(s => s != resourceName))
            {
                return null;
            }

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}