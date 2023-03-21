namespace Orc.NuGetExplorer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Catel.Data;

public sealed class NuGetFeed : ModelBase, ICloneable<NuGetFeed>, INotifyDataErrorInfo, IDataErrorInfo, INuGetSource
{
    private readonly IDictionary<string, string> _propertyNameToDataError = new Dictionary<string, string>();

    public NuGetFeed(string name, string source)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(source);

        Name = name;
        Source = source;
        Error = string.Empty;
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

    #region IDataErrorInfo
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
    #endregion

    #region INotifyDataErrorInfo
    public bool HasErrors => _propertyNameToDataError.Any();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName is null)
        {
            return Enumerable.Empty<string>();
        }

        if (_propertyNameToDataError.TryGetValue(propertyName, out var error))
        {
            return new[] { error };
        }

        return Enumerable.Empty<string>();
    }

    private void RaiseErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void ValidateAndRaiseErrorsChanged(string propertyName)
    {
        var error = this[propertyName];

        if (!_propertyNameToDataError.TryGetValue(propertyName, out var oldError))
        {
            oldError = string.Empty;
        }

        if (string.IsNullOrEmpty(error))
        {
            _propertyNameToDataError.Remove(propertyName);
        }
        else
        {
            _propertyNameToDataError[propertyName] = error;
        }

        if (!string.Equals(error, oldError))
        {
            RaiseErrorsChanged(propertyName);
        }
    }
    #endregion

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
        return IsNameValid && GetUriSource() is not null;
    }

    public bool IsLocal()
    {
        return GetUriSource()?.IsLoopback ?? false;
    }

    public Uri? GetUriSource()
    {
        try
        {
            return new Uri(Source);
        }
        catch (UriFormatException)
        {
            // Error = "Incorrect feed source can`t be recognized as Uri";
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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Source))
        {
            //reset verification
            VerificationResult = FeedVerificationResult.Unknown;
            ValidateAndRaiseErrorsChanged(e.PropertyName);
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
            ValidateAndRaiseErrorsChanged(e.PropertyName);
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