// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceSettingViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
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

            SuspendValidation = false;
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

        public bool IsValidatingFeeds { get; private set; }
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

            foreach (var packageSource in EditablePackageSources)
            {
#pragma warning disable 4014
                VerifyPackageSource(packageSource);
#pragma warning restore 4014
            }
        }

        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnModelPropertyChanged(sender, e);

            if (string.Equals(e.PropertyName, "Source"))
            {
                var selectedPackageSource = SelectedPackageSource;
                if (selectedPackageSource == null)
                {
                    return;
                }

                if (selectedPackageSource.IsValid == null)
                {
                    return;
                }

                selectedPackageSource.IsValid = false;
#pragma warning disable 4014
                VerifyPackageSource(selectedPackageSource);
#pragma warning restore 4014

            }
        }

        protected override async Task<bool> Save()
        {
            if (IsValidatingFeeds)
            {
                return false;
            }

            PackageSources = EditablePackageSources.Select(x => _packageSourceFactory.CreatePackageSource(x.Source, x.Name, x.IsEnabled, false)).ToArray();

            return await base.Save();
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);

            if (SelectedPackageSource == null || (SelectedPackageSource.IsValid ?? true))
            {
                return;
            }

            validationResults.Add(FieldValidationResult.CreateError("Source", "Package source '{0}' is invalid.", SelectedPackageSource.Source));
        }

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            base.ValidateBusinessRules(validationResults);

            if (EditablePackageSources != null && EditablePackageSources.Any(x => x.IsValid.HasValue && !x.IsValid.Value))
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("Some package sources are invalid."));
            }
        }

        private async Task VerifyPackageSource(EditablePackageSource packageSource)
        {
            if (packageSource == null || packageSource.IsValid == null)
            {
                return;
            }

            if (string.Equals(packageSource.Source, packageSource.PreviousSourceValue))
            {
                return;
            }

            packageSource.IsValid = null;

            string feedToValidate;
            bool isValid;

            IsValidatingFeeds = true;

            do
            {
                feedToValidate = packageSource.Source;

                var feedVerificationResult = await _nuGetFeedVerificationService.VerifyFeedAsync(feedToValidate, false);

                isValid = feedVerificationResult != FeedVerificationResult.Invalid && feedVerificationResult != FeedVerificationResult.Unknown;

            } while (!string.Equals(feedToValidate, packageSource.Source));

            IsValidatingFeeds = false;

            packageSource.PreviousSourceValue = packageSource.Source;
            packageSource.IsValid = isValid;

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
                Source = DefaultFeed,
                IsValid = true
            };

            EditablePackageSources.Add(packageSource);
            SelectedPackageSource = packageSource;

#pragma warning disable 4014
            VerifyPackageSource(packageSource);
#pragma warning restore 4014
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