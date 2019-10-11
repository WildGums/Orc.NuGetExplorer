// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectablePackageDetailsViewModelToPackageDetailsConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.old_NuGetExplorer.Converters
{
    using System;

    using Catel.MVVM.Converters;
    using ViewModels;

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