namespace Orc.NuGetExplorer.Example.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Catel.Data;

    [Serializable]
    public class PackageManagementEcho : ModelBase
    {
        protected PackageManagementEcho(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public PackageManagementEcho()
        {
            Lines = new ObservableCollection<string>();
        }

        public IList<string> Lines { get; private set; }
    }
}
