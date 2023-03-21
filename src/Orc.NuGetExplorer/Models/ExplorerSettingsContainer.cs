namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Catel.Data;
using NuGet.Configuration;

public sealed class ExplorerSettingsContainer : ModelBase, INuGetSettings
{
    private INuGetSource? _observedFeed;

    /// <summary>
    /// All feeds configured in application
    /// </summary>
    public List<NuGetFeed> NuGetFeeds { get; set; } = new List<NuGetFeed>();

    /// <summary>
    /// Feed currently used by explorer
    /// </summary>
    public INuGetSource? ObservedFeed
    {
        get => _observedFeed;
        set
        {
            if (_observedFeed != value)
            {
                var oldValue = _observedFeed;
                _observedFeed = value;
                RaisePropertyChanged(this, new PropertyChangedExtendedEventArgs<INuGetSource?>(oldValue, value));
            }
        }
    }

    public INuGetSource? DefaultFeed { get; set; }

    public bool IsPreReleaseIncluded { get; set; }

    public bool IsHideInstalled { get; set; }

    public bool IsRecommendedOnly { get; set; }

    public string SearchString { get; set; } = string.Empty;

    public void Clear()
    {
        NuGetFeeds.Clear();
        ObservedFeed = null;
    }

    /// <summary>
    /// Create and retrive all unique enabled package sources
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<PackageSource> GetAllPackageSources()
    {
        var feeds = NuGetFeeds.Where(x => x.IsSelected);

        if (!feeds.Any())
        {
            //then try to return all feeds, 'all' feed probably is selected
            feeds = NuGetFeeds.Where(x => x.IsEnabled);
        }

        return feeds.Select(x => new PackageSource(x.Source, x.Name, x.IsEnabled)).ToList();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(ObservedFeed)))
        {
            if (e is PropertyChangedExtendedEventArgs<INuGetSource?> args)
            {
                if (args.NewValue is not null)
                {
                    args.NewValue.IsSelected = true;
                }

                if (args.OldValue is not null)
                {
                    args.OldValue.IsSelected = false;
                }
            }
        }

        if (string.Equals(e.PropertyName, nameof(NuGetFeeds)))
        {
            ObservedFeed = NuGetFeeds.FirstOrDefault();
        }

        base.OnPropertyChanged(e);
    }
}