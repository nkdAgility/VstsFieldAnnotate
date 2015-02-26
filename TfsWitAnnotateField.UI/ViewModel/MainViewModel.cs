using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWitAnnotateField.UI.Infra;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace TfsWitAnnotateField.UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private TelemetryClient _telemetryClient;
        private TrackedRelayCommand _ConnectCommand;
        private TrackedRelayCommand _ClearErrorCommand;
        private TrackedRelayCommand _ExportCommand;
        private TfsTeamProjectCollection _SelectedTeamProjectCollection;
        private FieldDefinitionViewModel _SelectedFieldDefinition;
        private ObservableCollection<FieldDefinitionViewModel> _CheckedFieldDefinitions;
        private int _SelectedWorkItemId;
        private bool _IsErrorState;
        private WorkItemStore _store;
        private string _ErrorMessage;
        private ICollectionSelector _CollectionSelector;

        public bool IsConnected
        {
            get
            {
                return _SelectedTeamProjectCollection != null;
            }
        }

        public int SelectedWorkItemId
        {
            get
            {
                return _SelectedWorkItemId;
            }
            set
            {
                if (_SelectedWorkItemId != value)
                {
                    _SelectedWorkItemId = value;
                    this.RaisePropertyChanged("SelectedWorkItem");
                    this.RaisePropertyChanged("FieldDefinitions");
                    this.RaisePropertyChanged("FieldHistory");
                    this.RaisePropertyChanged("IsWorkItemSelected");
                    this.RaisePropertyChanged("SelectedWorkItem");
                }
            }
        }

        public ObservableCollection<FieldDefinitionViewModel> CheckedFieldDefinitions
        {
            get
            {
                return _CheckedFieldDefinitions;
            }
            set
            {
                if (_CheckedFieldDefinitions != value)
                {
                    _CheckedFieldDefinitions = GetFields(GetStore(_SelectedTeamProjectCollection));
                    this.RaisePropertyChanged("CheckedFieldDefinitions");
                    this.RaisePropertyChanged("FieldHistory");
                }
            }
        }

        public FieldDefinitionViewModel SelectedFieldDefinition
        {
            get
            {
                return _SelectedFieldDefinition;
            }
            set
            {
                if (_SelectedFieldDefinition != value)
                {
                    _SelectedFieldDefinition = value;
                    this.RaisePropertyChanged("SelectedFieldDefinition");
                    this.RaisePropertyChanged("FieldHistory");
                    //this.RaisePropertyChanged("FieldDefinitions");
                }
            }
        }

        public ReadOnlyCollection<string> FieldHistory
        {
            get
            {
                return new ReadOnlyCollection<string>(GetWorkItemFieldHistory(_SelectedWorkItemId));
            }

        }

        public WorkItem SelectedWorkItem
        {
            get
            {
                return GetWorkItem(_SelectedWorkItemId);
            }
        }

        public bool IsWorkItemSelected
        {
            get
            {
                return _SelectedWorkItemId != 0;
            }
        }

        public bool IsErrorState
        {
            get
            {
                return _IsErrorState;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                _ErrorMessage = value;
                _IsErrorState = true;
                this.RaisePropertyChanged("IsErrorState");
                this.RaisePropertyChanged("ErrorMessage");
            }
        }

        public bool IsFieldDefinitionSelected
        {
            get
            {
                return _SelectedFieldDefinition != null;
            }
        }

        public bool IsFieldDefinitionsChecked
        {
            get
            {
                var checkedDefinitions = from fieldDefinition in _CheckedFieldDefinitions
                                         where fieldDefinition.IsChecked == true
                                         select fieldDefinition;
                bool temp = checkedDefinitions.Count() != 0;
                return temp;
            }
        }

        public RelayCommand ConnectCommand
        {
            get
            {
                return _ConnectCommand;
            }
        }

        public RelayCommand ExportCommand
        {
            get
            {
                return _ExportCommand;
            }
        }

        public RelayCommand ClearErrorCommand
        {
            get
            {
                return _ClearErrorCommand;
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (IsConnected)
                    return string.Format("Connected to {0}", _SelectedTeamProjectCollection.Name);
                return "Connect to begin";
            }
        }

        public ReadOnlyCollection<FieldDefinitionViewModel> FieldDefinitions
        {
            get
            {
                if (IsInDesignMode || !IsConnected)
                {
                    // Code runs in Blend --> create design time data.

                    return new ReadOnlyCollection<FieldDefinitionViewModel>(new List<FieldDefinitionViewModel>());
                }
                else
                {
                    // Code runs "for real"
                    _CheckedFieldDefinitions = GetFields(GetStore(_SelectedTeamProjectCollection));
                    return new ReadOnlyCollection<FieldDefinitionViewModel>(_CheckedFieldDefinitions);
                }

            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ICollectionSelector collectionSelector)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            _telemetryClient = ((App)System.Windows.Application.Current).TemetryClient;
            _CollectionSelector = collectionSelector;
            _ConnectCommand = new TrackedRelayCommand(OnConnectCommand);
            _ClearErrorCommand = new TrackedRelayCommand(OnClearErrorCommand);
            _ExportCommand = new TrackedRelayCommand(OnExportCommand);
            Messenger.Default.Register<FieldDefinitionViewModel>(this, (action) => this.RaisePropertyChanged("FieldHistory"));

        }

        private WorkItemStore GetStore(TfsTeamProjectCollection collection)
        {
            if (_store == null)
                _store = collection.GetService<WorkItemStore>();
            return _store;
        }


        private ObservableCollection<FieldDefinitionViewModel> GetFields(WorkItemStore store)
        {
            if (IsConnected && IsWorkItemSelected && SelectedWorkItem != null)
            {

                ObservableCollection<FieldDefinitionViewModel> fieldDefinitions = new ObservableCollection<FieldDefinitionViewModel>();
                foreach (FieldDefinition f in SelectedWorkItem.Type.FieldDefinitions)
                {
                    FieldDefinitionViewModel temp = new FieldDefinitionViewModel(f);
                    fieldDefinitions.Add(temp);
                }
                return fieldDefinitions;
            }
            else
            {
                return new ObservableCollection<FieldDefinitionViewModel>();
            }
        }

        private void OnConnectCommand()
        {
            _telemetryClient.TrackEvent("Connect");
            
            _SelectedTeamProjectCollection = _CollectionSelector.SelectCollection();
            _SelectedTeamProjectCollection.EnsureAuthenticated();
            _telemetryClient.Context.User.Id = _SelectedTeamProjectCollection.AuthorizedIdentity.UniqueName;
            _telemetryClient.Context.User.AccountId = _SelectedTeamProjectCollection.Uri.ToString();
            _telemetryClient.Context.Properties["Collection"] = _SelectedTeamProjectCollection.Uri.ToString();
            _telemetryClient.Context.Properties["IsHostedServer"] = _SelectedTeamProjectCollection.IsHostedServer.ToString();
            _telemetryClient.Context.Properties["TimeZone"] = _SelectedTeamProjectCollection.TimeZone.ToString();
            _telemetryClient.Context.Properties["Culture"] = _SelectedTeamProjectCollection.Culture.ToString();
            if (_SelectedTeamProjectCollection != null)
            {
                this.RaisePropertyChanged("IsConnected");
                this.RaisePropertyChanged("ConnectedTo");
                this.RaisePropertyChanged("FieldDefinitions");
            }
        }

        private void OnClearErrorCommand()
        {
            _IsErrorState = false;
            _ErrorMessage = "";
            this.RaisePropertyChanged("IsErrorState");
            this.RaisePropertyChanged("ErrorMessage");
        }

        private void OnExportCommand()
        {
            ExportFieldHistory.ExportHistory(FieldHistory);
        }

        private WorkItem GetWorkItem(int id)
        {
            WorkItem result = null;
            if (IsConnected)
            {
                WorkItemStore store = GetStore(_SelectedTeamProjectCollection);
                try
                {
                    result = store.GetWorkItem(id);
                    OnClearErrorCommand();
                } catch (DeniedOrNotExistException) {
                    ErrorMessage = "That Work Item Id either does not exit or you do not have permission to view it";
                    result= null;
                }
            }
            return result;
        }

        private List<string> GetWorkItemFieldHistory(int id)
        {
            _telemetryClient.TrackEvent("TraceHistory");
            List<string> history = new List<string>();
            if (_CheckedFieldDefinitions != null)
            {
                foreach (var fieldDef in _CheckedFieldDefinitions)
                {
                    fieldDef.LastValue = null;
                }
            }
            if (IsConnected && IsWorkItemSelected && IsFieldDefinitionsChecked)
            {
                
                try
                {
                    IDictionary<String, String> properties = new Dictionary<String, String>();
                    IDictionary<String, Double> measurements = new Dictionary<String, Double>();
                    properties.Add("Collection", _SelectedTeamProjectCollection.Uri.ToString());

                    WorkItemStore store = GetStore(_SelectedTeamProjectCollection);
                    WorkItem result = store.GetWorkItem(id);
                    int revisionCount = result.Revisions.Count;
                    object value = null;

                    var checkedFields = from field in _CheckedFieldDefinitions
                                        where field.IsChecked == true
                                        select field;
                    //
                     measurements.Add("Revisions",revisionCount);
                    measurements.Add("Checked Fields",checkedFields.Count());
                    measurements.Add("Available Fields",result.Fields.Count);
                    //
                    foreach (Revision r in result.Revisions)
                    {
                        StringBuilder sBuilder = null;
                        

                        foreach (var fieldDefinitionViewModel in checkedFields)
                        {
                            value = r.Fields.GetById(fieldDefinitionViewModel.FieldDefinition.Id).Value;
                            if ((!(value == null)) && (!value.Equals(fieldDefinitionViewModel.LastValue)))
                            {
                                if (sBuilder == null)
                                {
                                    string changer = (string)r.Fields["System.ChangedBy"].Value;
                                    DateTime when = (DateTime)r.Fields["System.ChangedDate"].Value;
                                    sBuilder = new StringBuilder(string.Format("Revision: {0}\tDate: {1}\tUser: {2}", r.Index, when, changer));
                                }
                                sBuilder.Append(string.Format(Environment.NewLine + "  {0}: {1}", fieldDefinitionViewModel.Name, value));
                            }
                            fieldDefinitionViewModel.LastValue = value;

                        }
                        if (sBuilder != null)
                        {
                            history.Add(sBuilder.ToString());
                            sBuilder = null;
                        }
                    }
                    _telemetryClient.TrackEvent("TraceHistory_Load", properties, measurements);
                }
                catch (ValidationException ex)
                {
                    ErrorMessage = ex.Message;
                    history.Add(ex.Message);
                    throw ex;
                }
            }
            
            return history;
        }
    }
}