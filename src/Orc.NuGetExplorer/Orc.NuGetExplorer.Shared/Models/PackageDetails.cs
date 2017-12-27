// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetails.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    using Catel;
    using Catel.Data;
    using Catel.Logging;

    using NuGet;

    internal class PackageDetails : ModelBase, IPackageDetails
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _syncObj = new object();

        private bool _isValidating;

        private ValidationContext _validationContext;
        #endregion

        #region Constructors
        internal PackageDetails(IPackage package)
        {
            Argument.IsNotNull(() => package);

            Package = package;
            Version = package.Version.Version;
            Id = package.Id;
            Title = string.IsNullOrWhiteSpace(package.Title) ? package.Id : package.Title;
            FullName = package.GetFullName();
            Description = package.Description;
            IconUrl = package.IconUrl;

            Published = package.Published == null ? (DateTime?)null : package.Published.Value.LocalDateTime;
            SpecialVersion = package.Version.SpecialVersion;
            IsAbsoluteLatestVersion = package.IsAbsoluteLatestVersion;

            IsPrerelease = !string.IsNullOrWhiteSpace(SpecialVersion);
        }
        #endregion

        #region Properties
        public DateTime LastModified => _validationContext.LastModified;

        public long LastModifiedTicks => _validationContext.LastModifiedTicks;

        public bool HasWarnings => _validationContext.HasWarnings;

        public bool HasErrors => _validationContext.HasErrors;

        public string Id { get; }

        public string Title { get; }

        public IEnumerable<string> Authors => Package.Authors;

        DateTimeOffset? IPackageDetails.Published
        {
            get
            {
                var dataServicePackage = Package as DataServicePackage;
                return dataServicePackage == null ? Published : dataServicePackage.Published;
            }
        }

        public int? DownloadCount
        {
            get
            {
                var dataServicePackage = Package as DataServicePackage;
                return dataServicePackage == null ? null : (int?)dataServicePackage.DownloadCount;
            }
        }

        public string Dependencies
        {
            get
            {
                var dataServicePackage = Package as DataServicePackage;
                return dataServicePackage == null ? null : dataServicePackage.Dependencies;
            }
        }

        public bool? IsInstalled { get; set; }

        public string FullName { get; }

        public string Description { get; }

        public Uri IconUrl { get; }

        internal IPackage Package { get; }

        public DateTime? Published { get; }

        public Version Version { get; }

        public string SpecialVersion { get; }

        public bool IsAbsoluteLatestVersion { get; }

        public bool IsLatestVersion { get; private set; }

        public bool IsPrerelease { get; }
        #endregion

        #region Methods
        public void BeginValidation()
        {
            lock (_syncObj)
            {
                if (_isValidating)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("The object is already in Validating state");
                }

                _isValidating = true;
#pragma warning disable WPF1012 // Notify when property changes.
                _validationContext = new ValidationContext();
#pragma warning restore WPF1012 // Notify when property changes.
            }
        }

        public int GetValidationCount()
        {
            return _validationContext?.GetValidationCount() ?? 0;
        }

        public int GetValidationCount(object tag)
        {
            return _validationContext?.GetValidationCount(tag) ?? 0;
        }

        public List<IValidationResult> GetValidations()
        {
            return _validationContext?.GetValidations();
        }

        public List<IValidationResult> GetValidations(object tag)
        {
            return _validationContext?.GetValidations(tag);
        }

        public int GetWarningCount()
        {
            return _validationContext?.GetWarningCount() ?? 0;
        }

        public int GetWarningCount(object tag)
        {
            return _validationContext?.GetWarningCount(tag) ?? 0;
        }

        public List<IValidationResult> GetWarnings()
        {
            return _validationContext?.GetWarnings();
        }

        public List<IValidationResult> GetWarnings(object tag)
        {
            return _validationContext?.GetWarnings(tag);
        }

        public int GetErrorCount()
        {
            return _validationContext?.GetErrorCount() ?? 0;
        }

        public int GetErrorCount(object tag)
        {
            return _validationContext?.GetErrorCount(tag) ?? 0;
        }

        public List<IValidationResult> GetErrors()
        {
            return _validationContext?.GetErrors();
        }

        public List<IValidationResult> GetErrors(object tag)
        {
            return _validationContext?.GetErrors(tag);
        }

        public int GetFieldValidationCount()
        {
            return _validationContext?.GetFieldValidationCount() ?? 0;
        }

        public int GetFieldValidationCount(object tag)
        {
            return _validationContext?.GetFieldValidationCount(tag) ?? 0;
        }

        public List<IFieldValidationResult> GetFieldValidations()
        {
            return _validationContext?.GetFieldValidations();
        }

        public List<IFieldValidationResult> GetFieldValidations(object tag)
        {
            return _validationContext?.GetFieldValidations(tag);
        }

        public List<IFieldValidationResult> GetFieldValidations(string propertyName)
        {
            return _validationContext?.GetFieldValidations(propertyName);
        }

        public List<IFieldValidationResult> GetFieldValidations(string propertyName, object tag)
        {
            return _validationContext?.GetFieldValidations(propertyName, tag);
        }

        public int GetFieldWarningCount()
        {
            return _validationContext?.GetFieldWarningCount() ?? 0;
        }

        public int GetFieldWarningCount(object tag)
        {
            return _validationContext?.GetFieldWarningCount(tag) ?? 0;
        }

        public List<IFieldValidationResult> GetFieldWarnings()
        {
            return _validationContext?.GetFieldWarnings();
        }

        public List<IFieldValidationResult> GetFieldWarnings(object tag)
        {
            return _validationContext?.GetFieldWarnings(tag);
        }

        public List<IFieldValidationResult> GetFieldWarnings(string propertyName)
        {
            return _validationContext?.GetFieldWarnings(propertyName);
        }

        public List<IFieldValidationResult> GetFieldWarnings(string propertyName, object tag)
        {
            return _validationContext?.GetFieldWarnings(propertyName, tag);
        }

        public int GetFieldErrorCount()
        {
            return _validationContext?.GetFieldErrorCount() ?? 0;
        }

        public int GetFieldErrorCount(object tag)
        {
            return _validationContext?.GetFieldErrorCount(tag) ?? 0;
        }

        public List<IFieldValidationResult> GetFieldErrors()
        {
            return _validationContext?.GetFieldErrors();
        }

        public List<IFieldValidationResult> GetFieldErrors(object tag)
        {
            return _validationContext?.GetFieldErrors(tag);
        }

        public List<IFieldValidationResult> GetFieldErrors(string propertyName)
        {
            return _validationContext?.GetFieldErrors(propertyName);
        }

        public List<IFieldValidationResult> GetFieldErrors(string propertyName, object tag)
        {
            return _validationContext?.GetFieldErrors(propertyName, tag);
        }

        public int GetBusinessRuleValidationCount()
        {
            return _validationContext?.GetBusinessRuleValidationCount() ?? 0;
        }

        public int GetBusinessRuleValidationCount(object tag)
        {
            return _validationContext?.GetBusinessRuleValidationCount(tag) ?? 0;
        }

        public List<IBusinessRuleValidationResult> GetBusinessRuleValidations()
        {
            return _validationContext?.GetBusinessRuleValidations();
        }

        public List<IBusinessRuleValidationResult> GetBusinessRuleValidations(object tag)
        {
            return _validationContext?.GetBusinessRuleValidations(tag);
        }

        public int GetBusinessRuleWarningCount()
        {
            return _validationContext?.GetBusinessRuleWarningCount() ?? 0;
        }

        public int GetBusinessRuleWarningCount(object tag)
        {
            return _validationContext?.GetBusinessRuleWarningCount(tag) ?? 0;
        }

        public List<IBusinessRuleValidationResult> GetBusinessRuleWarnings()
        {
            return _validationContext?.GetBusinessRuleWarnings();
        }

        public List<IBusinessRuleValidationResult> GetBusinessRuleWarnings(object tag)
        {
            return _validationContext?.GetBusinessRuleWarnings(tag);
        }

        public int GetBusinessRuleErrorCount()
        {
            return _validationContext?.GetBusinessRuleErrorCount() ?? 0;
        }

        public int GetBusinessRuleErrorCount(object tag)
        {
            return _validationContext?.GetBusinessRuleErrorCount(tag) ?? 0;
        }

        public List<IBusinessRuleValidationResult> GetBusinessRuleErrors()
        {
            return _validationContext?.GetBusinessRuleErrors();
        }

        public List<IBusinessRuleValidationResult> GetBusinessRuleErrors(object tag)
        {
            return _validationContext?.GetBusinessRuleErrors(tag);
        }

        public void Add(IFieldValidationResult fieldValidationResult)
        {
            lock (_syncObj)
            {
                if (!_isValidating)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("The needs to enter in Validating state by calling BeginValidation first");
                }

                _validationContext.Add(fieldValidationResult);
            }
        }

        public void Remove(IFieldValidationResult fieldValidationResult)
        {
            lock (_syncObj)
            {
                if (!_isValidating)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("The needs to enter in Validating state by calling BeginValidation first");
                }

                _validationContext.Remove(fieldValidationResult);
            }
        }

        public void AddFieldValidationResult(IFieldValidationResult fieldValidationResult)
        {
            Add(fieldValidationResult);
        }

        public void RemoveFieldValidationResult(IFieldValidationResult fieldValidationResult)
        {
            Remove(fieldValidationResult);
        }

        public void Add(IBusinessRuleValidationResult businessRuleValidationResult)
        {
            lock (_syncObj)
            {
                if (!_isValidating)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("You must call BeginValidation first");
                }

                _validationContext.Add(businessRuleValidationResult);
            }
        }

        public void Remove(IBusinessRuleValidationResult businessRuleValidationResult)
        {
            lock (_syncObj)
            {
                if (!_isValidating)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("You must call BeginValidation first");
                }

                _validationContext.Remove(businessRuleValidationResult);
            }
        }

        public void AddBusinessRuleValidationResult(IBusinessRuleValidationResult businessRuleValidationResult)
        {
            Add(businessRuleValidationResult);
        }

        public void RemoveBusinessRuleValidationResult(IBusinessRuleValidationResult businessRuleValidationResult)
        {
            Remove(businessRuleValidationResult);
        }

        public void EndValidation()
        {
            lock (_syncObj)
            {
                _isValidating = false;
            }
        }
        #endregion
    }
}