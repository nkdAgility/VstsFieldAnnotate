using GalaSoft.MvvmLight.Command;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsWitAnnotateField.UI.Infra
{
    class TrackedRelayCommand : RelayCommand
    {
        private TelemetryClient _telemetryClient;
        private string _ComamndName;

        public TrackedRelayCommand(Action execute) : base(execute)
        {
            _ComamndName = string.Format("{0}:{1}", execute.Target.ToString(), execute.Method.Name);
            _telemetryClient = ((App)System.Windows.Application.Current).TemetryClient;
        }

        public TrackedRelayCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
            _telemetryClient = ((App)System.Windows.Application.Current).TemetryClient;
        }

        public override void Execute(object parameter)
        {
            Stopwatch sw = new Stopwatch();
            RequestTelemetry rt = new RequestTelemetry();
            rt.Name = string.Format("CommandExecute: {0}" , _ComamndName);
            rt.Timestamp = DateTime.Now;
            sw.Start();
            try
            {
                /////////////
                base.Execute(parameter);
                /////////////
                rt.ResponseCode = "100";
                rt.Success = true;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                rt.ResponseCode = "0";
                rt.Success = false;
            }
            finally
            {
                sw.Stop();
                rt.Duration = sw.Elapsed;
                _telemetryClient.TrackRequest(rt);
            }
            
        }

    }
}
