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
            var repoCategory = item as RepoCategory;
            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }

            switch (repoCategory.Name)
            {
                case RepoCategoryName.Installed:
                    return InstalledTemplate;
                case RepoCategoryName.Online:
                    return OnlineTemplate;
                case RepoCategoryName.Update:
                    return UpdateTemplate;
            }

            return base.SelectTemplate(item, container);
        }
        #endregion
    }
}