using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;

namespace TfsWitAnnotateField.UI.Infra
{
    class CollectionSelector : ICollectionSelector
    {
        public TfsTeamProjectCollection SelectCollection()
        {
            using (TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.NoProject, false))
            {
                DialogResult result = tpp.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return tpp.SelectedTeamProjectCollection;
                }
                return null;
            }
        }
    }
}
