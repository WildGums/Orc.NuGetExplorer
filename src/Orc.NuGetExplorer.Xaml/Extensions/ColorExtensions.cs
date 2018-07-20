// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows;
    using System.Windows.Media;

    internal static class ColorExtensions
    {
        #region Fields
        private static ResourceDictionary _accentColorResourceDictionary;
        #endregion

        #region Methods
        public static ResourceDictionary CreateAccentColorResourceDictionary(this Color color)
        {
            if (_accentColorResourceDictionary != null)
            {
                return _accentColorResourceDictionary;
            }
            var resourceDictionary = new ResourceDictionary();

            resourceDictionary.Add("AccentColor4", color.CalculateAccentColor4());
            resourceDictionary.Add("AccentColor", color);

            resourceDictionary.Add("AccentColorBrush4", new SolidColorBrush((Color) resourceDictionary["AccentColor4"]));
            resourceDictionary.Add("AccentColorBrush", new SolidColorBrush((Color) resourceDictionary["AccentColor"]));

            var application = Application.Current;
            var applicationResources = application.Resources;
            applicationResources.MergedDictionaries.Insert(0, resourceDictionary);

            _accentColorResourceDictionary = resourceDictionary;
            return applicationResources;
        }

        private static Color CalculateAccentColor4(this Color color)
        {
            return Color.FromArgb(51, color.R, color.G, color.B);
        }
        #endregion
    }
}