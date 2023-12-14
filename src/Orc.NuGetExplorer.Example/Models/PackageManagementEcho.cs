namespace Orc.NuGetExplorer.Example;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Catel.Data;

[Serializable]
public class PackageManagementEcho : ModelBase
{
    public PackageManagementEcho()
    {
        Lines = new ObservableCollection<string>();
    }

    public IList<string> Lines { get; private set; }
}
