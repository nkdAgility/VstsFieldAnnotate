using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsWitAnnotateField.UI.ViewModel
{
    public class FieldDefinitionViewModel : ViewModelBase
    {
        internal object LastValue { get; set; }

        private FieldDefinition _fieldDefinition = null;

        /// <summary>
        /// The <see cref="IsChecked" /> property's name.
        /// </summary>
        public const string IsCheckedPropertyName = "IsChecked";
        private bool _isChecked = false;

        /// <summary>
        /// Sets and gets the IsChecked property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }
           
                //RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
                Messenger.Default.Send<FieldDefinitionViewModel>(this);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = string.Empty;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                //RaisePropertyChanging(NamePropertyName);
                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        internal FieldDefinition FieldDefinition
        {
            get { return _fieldDefinition; }
        }

        public FieldDefinitionViewModel(FieldDefinition fieldDefinition)
        {
            _fieldDefinition = fieldDefinition;
            _name = _fieldDefinition.Name;
        }
    }
}
