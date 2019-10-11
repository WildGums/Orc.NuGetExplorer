namespace Orc.NuGetExplorer.Models
{
    using Catel.Data;
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class NuGetFeed : ModelBase, ICloneable<NuGetFeed>, IDataErrorInfo, INuGetSource
    {
        public NuGetFeed()
        {
            VerificationResult = FeedVerificationResult.Unknown;
            Error = String.Empty;
            IsActive = true;
        }

        public NuGetFeed(string name, string source)
        {
            Name = name;
            Source = source;
        }

        public string Name { get; set; }

        public string Source { get; set; }

        public bool IsActive { get; set; }

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

        [XmlIgnore]
        public bool IsVerified { get; private set; }

        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return $"{Name}\n{Source}";
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
                return String.IsNullOrEmpty(Source) ? null : new Uri(Source);
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
            return new NuGetFeed(
                this.Name, this.Source)
            {
                IsActive = this.IsActive,
                VerificationResult = this.VerificationResult,
                SerializationIdentifier = this.SerializationIdentifier
            };
        }

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

                return String.Empty;
            }
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
                IsAccessible = VerificationResult == FeedVerificationResult.Valid || VerificationResult == FeedVerificationResult.AuthorizationRequired;
                IsVerified = VerificationResult != FeedVerificationResult.Unknown;
            }
            if (e.PropertyName == nameof(Name))
            {
                IsNameValid = !String.IsNullOrEmpty(Name);
            }
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Called from configuration service
        /// </summary>
        public void Initialize()
        {
            IsNameValid = !String.IsNullOrEmpty(Name);
            IsAccessible = VerificationResult == FeedVerificationResult.Valid || VerificationResult == FeedVerificationResult.AuthorizationRequired;
            IsVerified = VerificationResult != FeedVerificationResult.Unknown;
        }
    }
}
