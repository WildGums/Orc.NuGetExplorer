// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceSettingControl.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Catel.MVVM.Views;

    public sealed partial class PackageSourceSettingControl
    {
        #region Constructors
        static PackageSourceSettingControl()
        {
            typeof(PackageSourceSettingControl).AutoDetectViewPropertiesToSubscribe();
        }

        public PackageSourceSettingControl()
        {
            CreateWarningAndErrorValidatorForViewModel = true;
            SkipSearchingForInfoBarMessageControl = false;
            AccentColorHelper.CreateAccentColorResourceDictionary();

            InitializeComponent();
        }
        #endregion

        #region DependencyProperty
        public static readonly DependencyProperty DefaultFeedProperty =
            DependencyProperty.Register("DefaultFeed", typeof(string), typeof(PackageSourceSettingControl), new PropertyMetadata(DefaultName.PackageSourceFeed));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultFeed
        {
            get { return (string)GetValue(DefaultFeedProperty); }
            set { SetValue(DefaultFeedProperty, value); }
        }

        public static readonly DependencyProperty DefaultSourceNameProperty =
            DependencyProperty.Register("DefaultSourceName", typeof(string), typeof(PackageSourceSettingControl), new PropertyMetadata(DefaultName.PackageSourceName));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultSourceName
        {
            get { return (string)GetValue(DefaultSourceNameProperty); }
            set { SetValue(DefaultSourceNameProperty, value); }
        }

        public static readonly DependencyProperty PackageSourcesProperty =
            DependencyProperty.Register("PackageSources", typeof(IEnumerable<IPackageSource>), typeof(PackageSourceSettingControl), new PropertyMetadata(Enumerable.Empty<IPackageSource>()));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public IEnumerable<IPackageSource> PackageSources
        {
            get { return (IEnumerable<IPackageSource>)GetValue(PackageSourcesProperty); }
            set { SetValue(PackageSourcesProperty, value); }
        }
        #endregion
    }
}