// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectablePackageDetailsViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    using Catel;
    using Catel.MVVM;

    [Serializable]
    public sealed class SelectablePackageDetailsViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackageDetails _packageDetails;
        #endregion

        #region Constructors
        public SelectablePackageDetailsViewModel(IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            _packageDetails = packageDetails;
            SelectPackageVersionCommand = new Command<string>(Execute);
        }
        #endregion

        #region Properties
        public Uri IconUrl => _packageDetails?.IconUrl;

        public override string Title => _packageDetails?.Title;

        public IList<string> AvailableVersions => _packageDetails?.AvailableVersions;

        public ICommand SelectPackageVersionCommand { get; }

        public bool? IsInstalled
        {
            get => _packageDetails?.IsInstalled;
            set
            {
                if (_packageDetails != null)
                {
                    _packageDetails.IsInstalled = value;
                }
            }
        }

        public string Description => _packageDetails?.Description;

        public string SelectedVersion
        {
            get => _packageDetails?.SelectedVersion;
            set
            {
                if (_packageDetails != null)
                {
                    _packageDetails.SelectedVersion = value;
                }
            }
        }

        public IPackageDetails PackageDetails => _packageDetails;

        #endregion

        #region Methods
        private void Execute(string version)
        {
            SelectedVersion = version;
        }
        #endregion
    }
}
