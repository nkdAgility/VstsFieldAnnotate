using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using TfsWitAnnotateField.UI.Infra;

namespace TfsWitAnnotateField.UI.Tests
{
    class MockCollectionSelector : ICollectionSelector
    {
        public TfsTeamProjectCollection SelectCollection()
        {
            return new TfsTeamProjectCollection(new Uri("https://mrhinsh.tfspreview.com/"), new TfsClientCredentials(new SimpleWebTokenCredential("", "")));
        }
    }
}
