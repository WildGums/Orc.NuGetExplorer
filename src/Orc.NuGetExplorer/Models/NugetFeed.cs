namespace Orc.NuGetExplorer.Models
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using Catel.Data;

    public sealed class NuGetFeed : ModelBase, ICloneable<NuGetFeed>, IDataErrorInfo, INuGetSource
    {
        public NuGetFeed()
        {
            VerificationResult = FeedVerificationResult.Unknown;
            Error = string.Empty;
            IsEnabled = true;
        }

        public NuGetFeed(string name, string source)
        {
            Name = name;
            Source = source;
        }

        public NuGetFeed(string name, string source, bool isEnabled) : this(name, source)
        {
            IsEnabled = isEnabled;
        }

        public NuGetFeed(string name, string source, bool isEnabled, bool isOfficial) : this(name, source, isEnabled)
        {
            IsOfficial = isOfficial;
        }

        public string Name { get; set; }

        public string Source { get; set; }

        public bool IsEnabled { get; set; }

        [XmlIgnore]
        public bool IsVerifiedNow { get; set; }

        [XmlIgnore]
        public Guid SerializationIdentifier { get; set; }

        [XmlIgnore]
        public FeedVerificationResult VerificationResult { get; set; }

        [XmlIgnore]
        public bool IsNameValid { get; private set; }

        [XmlIgnore]
        public bool IsAccessible { get; set; }

        public bool IsRestricted { get; set; }

        [XmlIgnore]
        public bool IsVerified { get; private set; }

        public bool IsSelected { get; set; }

        public bool IsOfficial { get; set; }

        public string Error { get; private set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Name):

                        if (!IsNameValid)
                        {
                            return "Feed name cannot be empty";
                        }
                        break;

                    case nameof(Source):

                        if (GetUriSource() == null)
                        {
                            return "Incorrect feed source can`t be recognized as Uri";
                        }
                        break;
                }

                return string.Empty;
            }
        }

        public override string ToString()
        {
            return $"{Name} {Source}";
        }

        public void ForceCancelEdit()
        {
            IEditableObject eo = this;
            eo.CancelEdit();
        }

        public void ForceEndEdit()
        {
            IEditableObject eo = this;
            eo.EndEdit();
        }

        public bool IsValid()
        {
            return IsNameValid && GetUriSource() != null;
        }

        public bool IsLocal()
        {
            return GetUriSource()?.IsLoopback ?? false;
        }

        public Uri GetUriSource()
        {
            try
            {
                return string.IsNullOrEmpty(Source) ? null : new Uri(Source);
            }
            catch (UriFormatException)
            {
                Error = "Incorrect feed source can`t be recognized as Uri";
                return null;
            }
        }

        public PackageSourceWrapper GetPackageSource()
        {
            return new PackageSourceWrapper(Source);
        }

        public NuGetFeed Clone()
        {
            return new NuGetFeed(Name, Source)
            {
                IsEnabled = IsEnabled,
                VerificationResult = VerificationResult,
                SerializationIdentifier = SerializationIdentifier
            };
        }


        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Source))
            {
                //reset verification
                VerificationResult = FeedVerificationResult.Unknown;
            }
            if (e.PropertyName == nameof(VerificationResult))
            {
                IsAccessible = VerificationResult == FeedVerificationResult.Valid;
                IsVerified = VerificationResult != FeedVerificationResult.Unknown;
                IsRestricted = IsVerified && 
                    (VerificationResult == FeedVerificationResult.AuthenticationRequired || VerificationResult == FeedVerificationResult.AuthorizationRequired);
            }
            if (e.PropertyName == nameof(Name))
            {
                IsNameValid = !string.IsNullOrEmpty(Name);
            }
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Called from configuration service
        /// </summary>
        public void Initialize()
        {
            IsNameValid = !string.IsNullOrEmpty(Name);
            IsAccessible = VerificationResult == FeedVerificationResult.Valid;
            IsVerified = VerificationResult != FeedVerificationResult.Unknown;
            IsRestricted = IsVerified &&
                (VerificationResult == FeedVerificationResult.AuthenticationRequired || VerificationResult == FeedVerificationResult.AuthorizationRequired);
        }
    }
}
