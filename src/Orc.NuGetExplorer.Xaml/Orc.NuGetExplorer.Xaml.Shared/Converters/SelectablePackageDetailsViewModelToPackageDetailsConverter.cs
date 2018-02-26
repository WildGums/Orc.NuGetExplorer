// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectablePackageDetailsViewModelToPackageDetailsConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer.Converters
{
    using System;

    using Catel.MVVM.Converters;

    using Orc.NuGetExplorer.ViewModels;

    public class SelectablePackageDetailsViewModelToPackageDetailsConverter : ValueConverterBase<IPackageDetails, SelectablePackageDetailsViewModel>
    {
        protected override object Convert(IPackageDetails value, Type targetType, object parameter)
        {
            return null;
        }

        protected override object ConvertBack(SelectablePackageDetailsViewModel value, Type targetType, object parameter)
        {
            return value.PackageDetails;
        }
    }
}