// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationTemplateSelector.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.TemplateSelectors
{
    using System.Windows;
    using System.Windows.Controls;

    public class NavigationTemplateSelector : DataTemplateSelector
    {
        #region Properties
        public DataTemplate InstalledTemplate { get; set; }
        public DataTemplate OnlineTemplate { get; set; }
        public DataTemplate UpdateTemplate { get; set; }
        #endregion

        #region Methods
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var navigationGroup = item as NavigationItemsGroup;
            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }

            switch (navigationGroup.Name)
            {
                case PackageGroups.Installed:
                    return InstalledTemplate;
                case PackageGroups.Online:
                    return OnlineTemplate;
                case PackageGroups.Update:
                    return UpdateTemplate;
            }

            return base.SelectTemplate(item, container);
        }
        #endregion
    }
}