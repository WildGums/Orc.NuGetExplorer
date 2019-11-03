namespace Orc.NuGetExplorer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Data;
    using NuGet.Configuration;

    public class ExplorerSettingsContainer : ModelBase, INuGetSettings
    {
        /// <summary>
        /// All feeds configured in application
        /// </summary>
        public List<NuGetFeed> NuGetFeeds { get; set; } = new List<NuGetFeed>();

        /// <summary>
        /// Feed currently used by explorer
        /// </summary>
        public INuGetSource ObservedFeed { get; set; }

        public bool IsPreReleaseIncluded { get; set; }

        public string SearchString { get; set; } = String.Empty;

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

            if(!feeds.Any())
            {
                //then try to return all feeds, 'all' feed probably is selected
                feeds = NuGetFeeds.Where(x => x.IsEnabled);
            }

            return feeds.Select(x => new PackageSource(x.Source)).ToList();
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(ObservedFeed)))
            {
                var source = e.NewValue as INuGetSource;

                if (source != null)
                {
                    source.IsSelected = true;
                }

                var oldSelected = e.OldValue as INuGetSource;

                if (oldSelected != null)
                {
                    oldSelected.IsSelected = false;
                }
            }

            if (string.Equals(e.PropertyName, nameof(NuGetFeeds)))
            {
                ObservedFeed = NuGetFeeds.FirstOrDefault();
            }

            base.OnPropertyChanged(e);
        }
    }
}
