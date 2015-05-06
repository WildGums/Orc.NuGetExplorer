// --------------------------------------------------------------------------------------------------------------------
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
        private readonly INuGetFeedVerificationService _nuGetFeedVerificationService;
        private readonly IPackageSourceFactory _packageSourceFactory;
        private FeedVerificationResult _feedVerificationResult = FeedVerificationResult.Unknown;
        private bool _isSourceVerified;
        private bool _isVerifying;
        private EditablePackageSource _sourceToVerify;
        #endregion

        #region Constructors
        public PackageSourceSettingViewModel(INuGetFeedVerificationService nuGetFeedVerificationService, IPackageSourceFactory packageSourceFactory)
        {
            Argument.IsNotNull(() => nuGetFeedVerificationService);
            Argument.IsNotNull(() => packageSourceFactory);

            _nuGetFeedVerificationService = nuGetFeedVerificationService;
            _packageSourceFactory = packageSourceFactory;

            Add = new TaskCommand(OnAddExecute);
            Remove = new TaskCommand(OnRemoveExecute, OnRemoveCanExecute);
        }
        #endregion

        #region Properties
        public IList<EditablePackageSource> EditablePackageSources { get; private set; }
        public IEnumerable<IPackageSource> PackageSources { get; set; }

        [Model(SupportIEditableObject = false)]
        [Expose("Name")]
        [Expose("Source")]
        public EditablePackageSource SelectedPackageSource { get; set; }

        public string DefaultFeed { get; set; }
        public string DefaultSourceName { get; set; }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            if (PackageSources != null)
            {
                OnPackageSourcesChanged();
            }

            await base.Initialize();
        }

        private void OnPackageSourcesChanged()
        {
            EditablePackageSources = new FastObservableCollection<EditablePackageSource>(PackageSources.Select(x =>
                new EditablePackageSource
                {
                    IsEnabled = x.IsEnabled,
                    Name = x.Name,
                    Source = x.Source
                }));
        }

        private void OnSourceChanged()
        {
            if (SelectedPackageSource == null)
            {
                return;
            }

            _isSourceVerified = false;
        }

        protected override async Task<bool> Save()
        {
            PackageSources = EditablePackageSources.Select(x => _packageSourceFactory.CreatePackageSource(x.Source, x.Name, x.IsEnabled, false));

            return await base.Save();
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);

            if (EditablePackageSources != null)
            {
                var errorsCount = EditablePackageSources.
                    Select(packageSource => packageSource.GetValidationContext()).
                    Select(validationContext => validationContext.GetErrorCount()).
                    Sum();

                if (errorsCount > 0)
                {
                    validationResults.Add(FieldValidationResult.CreateError("EditablePackageSources", "Some package sources are invalid."));
                }
            }

            if (SelectedPackageSource == null || _isSourceVerified)
            {
                return;
            }

            if (_isSourceVerified)
            {
                return;
            }

            ValidatePackageSource(SelectedPackageSource);
            validationResults.Add(FieldValidationResult.CreateWarning("Source", string.Format("Verification of package source '{0}'", SelectedPackageSource.Source)));
            validationResults.Add(FieldValidationResult.CreateError("EditablePackageSources", "Some package sources are not verified."));
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
                        feed.AddFieldValidationResult(FieldValidationResult.CreateError("Source", string.Format("The package source '{0}' is invalid.", feed.Source)), true);
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

            EditablePackageSources.Add(packageSource);
            SelectedPackageSource = packageSource;
        }

        public TaskCommand Remove { get; private set; }

        private async Task OnRemoveExecute()
        {
            if (SelectedPackageSource == null)
            {
                return;
            }

            var index = EditablePackageSources.IndexOf(SelectedPackageSource);
            if (index < 0)
            {
                return;
            }

            EditablePackageSources.RemoveAt(index);

            if (index < EditablePackageSources.Count)
            {
                SelectedPackageSource = EditablePackageSources[index];
            }
            else
            {
                SelectedPackageSource = EditablePackageSources.LastOrDefault();
            }
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedPackageSource != null;
        }
        #endregion
    }
}