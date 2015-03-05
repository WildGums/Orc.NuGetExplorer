// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetails.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Data;
    using NuGet;

    public class PackageDetails : ModelBase
    {
        #region Constructors
        public PackageDetails(IPackage package)
        {
            Argument.IsNotNull(() => package);

            Package = package;

            Id = package.Id;
            Title = package.GetFullName();
            Summary = package.Description;
            IconUrl = package.IconUrl;
        }
        #endregion

        #region Properties
        public string Id { get; private set; }
        public string Title { get; private set; }
        public string Summary { get; private set; }
        public Uri IconUrl { get; private set; }
        public IPackage Package { get; private set; }
        #endregion
    }
}