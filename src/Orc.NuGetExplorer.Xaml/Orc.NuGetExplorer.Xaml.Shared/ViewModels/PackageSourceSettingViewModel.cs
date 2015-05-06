﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceSettingViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.Fody;
    using Catel.MVVM;

    internal class PackageSourceSettingViewModel : ViewModelBase
    {
        #region Fields
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly INuGetFeedVerificationService _nuGetFeedVerificationService;
        private FeedVerificationResult _feedVerificationResult = FeedVerificationResult.Unknown;
        private bool _isSourceVerified;
        private bool _isVerifying;
        private EditablePackageSource _sourceToVerify;
        #endregion

        #region Constructors
        public PackageSourceSettingViewModel(INuGetConfigurationService nuGetConfigurationService, INuGetFeedVerificationService nuGetFeedVerificationService)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => nuGetFeedVerificationService);

            _nuGetConfigurationService = nuGetConfigurationService;
            _nuGetFeedVerificationService = nuGetFeedVerificationService;

            Add = new TaskCommand(OnAddExecute);
            Remove = new TaskCommand(OnRemoveExecute, OnRemoveCanExecute);
        }
        #endregion

        #region Properties
        public IList<EditablePackageSource> PackageSources { get; private set; }

        [Model]
        [Expose("Name")]
        [Expose("Source")]
        public EditablePackageSource SelectedPackageSource { get; set; }

        public string DefaultFeed { get; set; }
        public string DefaultSourceName { get; set; }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            var loadPackageSources = _nuGetConfigurationService.LoadPackageSources();

            PackageSources = new FastObservableCollection<EditablePackageSource>(loadPackageSources.Select(x =>
                new EditablePackageSource
                {
                    IsEnabled = x.IsEnabled,
                    Name = x.Name,
                    Source = x.Source
                }));

            await base.Initialize();
        }

        private void OnSourceChanged()
        {
            if (SelectedPackageSource == null)
            {
                return;
            }

            _isSourceVerified = false;
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);

            if (SelectedPackageSource == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedPackageSource.Source))
            {
                var fieldValidationResult = FieldValidationResult.CreateError("Source", "The package source is required.");
                validationResults.Add(fieldValidationResult);
                SelectedPackageSource.AddFieldValidationResult(fieldValidationResult, true);
                return;
            }

            if (!_isSourceVerified)
            {
                ValidatePackageSource(SelectedPackageSource);
                validationResults.Add(FieldValidationResult.CreateWarning("Source", "Feed verification in progress."));
                return;
            }
        }

        private void ValidatePackageSource(EditablePackageSource packageSource)
        {
            _sourceToVerify = packageSource;

            if (_isVerifying)
            {
                return;
            }

#pragma warning disable 4014
            VerifyPackageSource();
#pragma warning restore 4014
        }

        private async Task VerifyPackageSource()
        {
            using (new DisposableToken(null, x => _isVerifying = true, x => _isVerifying = false))
            {
                while (_sourceToVerify != null)
                {
                    var feed = _sourceToVerify;
                    _sourceToVerify = null;
                    _feedVerificationResult = await _nuGetFeedVerificationService.VerifyFeedAsync(feed.Source, false);
                    if (_feedVerificationResult == FeedVerificationResult.Invalid || _feedVerificationResult == FeedVerificationResult.Unknown)
                    {
                        feed.AddFieldValidationResult(FieldValidationResult.CreateError("Source", "The package source is invalid."), true);
                    }
                }

                _isSourceVerified = true;
            }

            ValidateViewModel(true);
        }
        #endregion

        #region Commands
        public TaskCommand Add { get; private set; }

        private async Task OnAddExecute()
        {
            var packageSource = new EditablePackageSource
            {
                IsEnabled = true,
                Name = DefaultSourceName,
                Source = DefaultFeed
            };

            PackageSources.Add(packageSource);
            SelectedPackageSource = packageSource;
        }

        public TaskCommand Remove { get; private set; }

        private async Task OnRemoveExecute()
        {
            if (SelectedPackageSource == null)
            {
                return;
            }

            var index = PackageSources.IndexOf(SelectedPackageSource);
            if (index < 0)
            {
                return;
            }

            PackageSources.RemoveAt(index);

            if (index < PackageSources.Count)
            {
                SelectedPackageSource = PackageSources[index];
            }
            else
            {
                SelectedPackageSource = PackageSources.LastOrDefault();
            }
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedPackageSource != null;
        }
        #endregion
    }
}