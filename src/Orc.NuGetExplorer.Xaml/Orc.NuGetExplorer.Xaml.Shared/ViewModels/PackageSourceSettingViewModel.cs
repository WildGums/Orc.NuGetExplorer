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
            MoveUp = new TaskCommand(OnMoveUpExecute, OnMoveUpCanExecute);
            MoveDown = new TaskCommand(OnMoveDownExecute, OnMoveDownCanExecute);

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

            VerifyAll();
        }        

        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnModelPropertyChanged(sender, e);
            if (string.Equals(e.PropertyName, "Name"))
            {
                VerifyAll();
            }

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
            if (EditablePackageSources != null && EditablePackageSources.Any(x => x.IsValid == null))
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

            if (!(SelectedPackageSource.IsValidName ?? false))
            {
                validationResults.Add(FieldValidationResult.CreateError("Name", "Package source name '{0}' is empty or not unique.", SelectedPackageSource.Name));
            }

            if (!(SelectedPackageSource.IsValidSource ?? false))
            {
                validationResults.Add(FieldValidationResult.CreateError("Source", "Package source '{0}' is invalid.", SelectedPackageSource.Source));
            }            
        }

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            base.ValidateBusinessRules(validationResults);

            if (EditablePackageSources != null && EditablePackageSources.Any(x => x.IsValid.HasValue && !x.IsValid.Value))
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("Some package sources are invalid."));
            }
        }

        private async Task VerifyPackageSource(EditablePackageSource packageSource, bool force = false)
        {
            if (packageSource == null || packageSource.IsValid == null)
            {
                return;
            }

            if (!force && string.Equals(packageSource.Source, packageSource.PreviousSourceValue))
            {
                return;
            }

            packageSource.IsValid = null;

            string feedToValidate;
            string nameToValidate;
            bool isValidUrl;
            bool isValidName;

            do
            {
                feedToValidate = packageSource.Source;
                nameToValidate = packageSource.Name;

                var namesCount = EditablePackageSources.Count(x => string.Equals(nameToValidate, x.Name));

                isValidName = !string.IsNullOrWhiteSpace(nameToValidate) && namesCount == 1;

                var feedVerificationResult = await _nuGetFeedVerificationService.VerifyFeedAsync(feedToValidate, false);

                isValidUrl = feedVerificationResult != FeedVerificationResult.Invalid && feedVerificationResult != FeedVerificationResult.Unknown;

            } while (!string.Equals(feedToValidate, packageSource.Source) 
                && !string.Equals(nameToValidate, packageSource.Name));

            packageSource.PreviousSourceValue = packageSource.Source;
            packageSource.IsValidSource = isValidUrl;
            packageSource.IsValidName = isValidName;

            ValidateViewModel(true);
        }

        private bool CanMoveToStep(int step)
        {
            var selectedPackageSource = SelectedPackageSource;

            if (selectedPackageSource == null)
            {
                return false;
            }

            var editablePackageSources = EditablePackageSources;

            var index = editablePackageSources.IndexOf(selectedPackageSource);

            var newIndex = index + step;

            return newIndex >= 0 && newIndex < editablePackageSources.Count;
        }

        private void MoveToStep(int step)
        {
            var selectedPackageSource = SelectedPackageSource;

            if (selectedPackageSource == null)
            {
                return;
            }

            var editablePackageSources = EditablePackageSources;

            var index = editablePackageSources.IndexOf(selectedPackageSource);

            EditablePackageSources.RemoveAt(index);
            EditablePackageSources.Insert(index + step, selectedPackageSource);

            SelectedPackageSource = selectedPackageSource;
        }

        private void VerifyAll()
        {
            foreach (var packageSource in EditablePackageSources)
            {
#pragma warning disable 4014
                VerifyPackageSource(packageSource, true);
#pragma warning restore 4014
            }
        }
        #endregion

        #region Commands
        public TaskCommand MoveUp { get; private set; }

        private async Task OnMoveUpExecute()
        {
            MoveToStep(-1);
        }

        private bool OnMoveUpCanExecute()
        {
            return CanMoveToStep(-1);
        }

        public TaskCommand MoveDown { get; private set; }

        private async Task OnMoveDownExecute()
        {
            MoveToStep(1);
        }

        private bool OnMoveDownCanExecute()
        {
            return CanMoveToStep(1);
        }

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

            VerifyAll();
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

            VerifyAll();
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedPackageSource != null;
        }
        #endregion
    }
}