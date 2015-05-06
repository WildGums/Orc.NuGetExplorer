// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceSettingControl.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

    public sealed partial class PackageSourceSettingControl
    {
        #region Constructors
        static PackageSourceSettingControl()
        {
            typeof (PackageSourceSettingControl).AutoDetectViewPropertiesToSubscribe();
        }

        public PackageSourceSettingControl()
        {
            this.InitializeComponent();
        }
        #endregion

        #region DependencyProperty
        public static readonly DependencyProperty DefaultFeedProperty =
            DependencyProperty.Register("DefaultFeed", typeof (string), typeof (PackageSourceSettingControl), new PropertyMetadata(DefaultName.PackageSourceFeed));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultFeed
        {
            get { return (string) GetValue(DefaultFeedProperty); }
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
        #endregion
    }
}