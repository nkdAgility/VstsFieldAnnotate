using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;

namespace TfsWitAnnotateField.UI.Infra
{
    public interface ICollectionSelector
    {
        TfsTeamProjectCollection SelectCollection();
    }
}
