// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManagementEcho.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Data;

    public class PackageManagementEcho : ModelBase
    {
        #region Constructors
        public PackageManagementEcho()
        {
            Lines = new ObservableCollection<string>();
        }
        #endregion

        #region Properties
        public IList<string> Lines { get; private set; }
        #endregion
    }
}