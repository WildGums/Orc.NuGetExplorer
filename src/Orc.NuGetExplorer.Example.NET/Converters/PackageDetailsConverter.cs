// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class PackageDetailsConverter : ValueConverterBase
    {
        #region Methods
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var packageDetails = value as IPackageDetails;
            if (packageDetails == null)
            {
                return null;
            }

            return packageDetails.FullName;
        }
        #endregion
    }
}