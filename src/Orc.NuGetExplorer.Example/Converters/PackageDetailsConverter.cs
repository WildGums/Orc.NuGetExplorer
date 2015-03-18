// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsConverter.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer.Example.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class PackageDetailsConverter : ValueConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var packageDetails = value as IPackageDetails;
            if (packageDetails == null)
            {
                return null;
            }

            return packageDetails.FullName;
        }
    }
}