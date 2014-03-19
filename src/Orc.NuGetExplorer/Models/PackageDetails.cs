// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetails.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Models
{
    using System.Windows.Media.Imaging;
    using Catel;
    using Catel.Data;
    using NuGet;

    public class PackageDetails : ModelBase
    {
        #region Constructors
        public PackageDetails(IPackage package)
        {
            Argument.IsNotNull(() => package);

            Id = package.Id;
            Title = package.Title;
            Summary = package.Summary;

            Package = package;
        }
        #endregion

        #region Properties
        public string Id { get; private set; }

        public string Title { get; private set; }

        public string Summary { get; private set; }

        public BitmapSource Icon { get; private set; }

        public IPackage Package { get; private set; }
        #endregion
    }
}