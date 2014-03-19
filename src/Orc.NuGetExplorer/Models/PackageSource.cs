// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSource.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Models
{
    using Catel.Data;

    public class PackageSource : ModelBase
    {
        public PackageSource()
        {
            
        }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}