// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationTemplateSelector.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.TemplateSelectors
{
    using System;
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
            var repoCategory = item as RepoCategory;
            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }

            if (repoCategory != null)
            {
                switch ((RepoCategoryType)Enum.Parse(typeof(RepoCategoryType), repoCategory.Name))
                {
                    case RepoCategoryType.Installed:
                        return InstalledTemplate;
                    case RepoCategoryType.Online:
                        return OnlineTemplate;
                    case RepoCategoryType.Update:
                        return UpdateTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
        #endregion
    }
}