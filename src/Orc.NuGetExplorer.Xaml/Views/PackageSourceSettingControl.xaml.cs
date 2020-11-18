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

            InitializeComponent();
        }
        #endregion

        #region DependencyProperty
        /// <summary>
        /// Identifies the <see cref="DefaultFeed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultFeedProperty =
            DependencyProperty.Register(nameof(DefaultFeed), typeof(string), typeof(PackageSourceSettingControl), new PropertyMetadata(DefaultName.PackageSourceFeed));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultFeed
        {
            get { return (string)GetValue(DefaultFeedProperty); }
            set { SetValue(DefaultFeedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CanReset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanResetProperty =
            DependencyProperty.Register(nameof(CanReset), typeof(bool), typeof(PackageSourceSettingControl), new PropertyMetadata(false));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public bool CanReset
        {
            get { return (bool)GetValue(CanResetProperty); }
            set { SetValue(CanResetProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DefaultSourceName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultSourceNameProperty =
            DependencyProperty.Register(nameof(DefaultSourceName), typeof(string), typeof(PackageSourceSettingControl), new PropertyMetadata(DefaultName.PackageSourceName));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultSourceName
        {
            get { return (string)GetValue(DefaultSourceNameProperty); }
            set { SetValue(DefaultSourceNameProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PackageSources"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PackageSourcesProperty =
            DependencyProperty.Register(nameof(PackageSources), typeof(IEnumerable<IPackageSource>), typeof(PackageSourceSettingControl), new PropertyMetadata(Enumerable.Empty<IPackageSource>()));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public IEnumerable<IPackageSource> PackageSources
        {
            get { return (IEnumerable<IPackageSource>)GetValue(PackageSourcesProperty); }
            set { SetValue(PackageSourcesProperty, value); }
        }
        #endregion
    }
}
