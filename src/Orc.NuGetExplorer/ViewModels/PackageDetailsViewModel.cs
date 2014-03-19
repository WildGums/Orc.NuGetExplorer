// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using NuGet;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Services;

    public class PackageDetailsViewModel : ViewModelBase
    {
        public PackageDetailsViewModel(PackageDetails package)
        {
            Argument.IsNotNull(() => package);

            Package = package;
        }

        public override string Title
        {
            get { return Package.Title; }
        }

        [Model(SupportIEditableObject = false)]
        [Expose("Summary")]
        [Expose("Icon")]
        public PackageDetails Package { get; private set; }
    }
}