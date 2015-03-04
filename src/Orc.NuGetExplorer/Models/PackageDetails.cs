// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetails.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
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
            Title = package.GetFullName();
            Summary = package.Description;

            Package = package;
            if (package.IconUrl != null)
            {
                Icon = new BitmapImage(package.IconUrl);
            }
            else
            {
                Icon = new BitmapImage(new Uri(@"http://www.nuget.org/Content/Images/packageDefaultIcon.png"));
            }
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