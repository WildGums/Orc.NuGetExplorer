// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.MVVM;
    using NuGet;

    public class PackageDetailsViewModel : ViewModelBase
    {
        public PackageDetailsViewModel(IServerPackageMetadata package)
        {
            Argument.IsNotNull(() => package);

            Package = package;
        }

        public IServerPackageMetadata Package { get; private set; }
    }
}