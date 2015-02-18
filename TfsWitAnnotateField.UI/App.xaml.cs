using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TfsWitAnnotateField.UI.Infra;

namespace TfsWitAnnotateField.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        AssemblyLoader assemblyLoader;
        TelemetryClient telemetryClient;

        public TelemetryClient TemetryClient { get { return telemetryClient; } }

        public App() : base()
        {
            telemetryClient = new TelemetryClient();
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            assemblyLoader = new AssemblyLoader();
            assemblyLoader.BindAssemblyResolveEventHandler();
            TemetryClient.Context.Properties["DEBUG"] = Debugger.IsAttached.ToString();
            //TemetryClient.Context.Properties["CustomTrackingProperty"] = "OCT2014";

        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            TemetryClient.TrackException(e.Exception);
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            //Application curApp = Application.Current;
            //curApp.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            TemetryClient.TrackEvent("OnExit");
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            TemetryClient.TrackEvent("OnStartup");
            base.OnStartup(e);
        }
    }
}
