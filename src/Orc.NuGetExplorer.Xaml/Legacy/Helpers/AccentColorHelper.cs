// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccentColorHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows;
    using System.Windows.Media;
    using Orc.Controls;

    internal static class AccentColorHelper
    {
        #region Fields
        private static bool IsAccentColorResourceDictionaryCreated;
        #endregion

        #region Methods
        public static void CreateAccentColorResourceDictionary()
        {
            if (IsAccentColorResourceDictionaryCreated)
            {
                return;
            }

            var accentColor = GetAccentColorBrush().Color;

            accentColor.CreateAccentColorResourceDictionary();

            IsAccentColorResourceDictionaryCreated = true;
        }

        private static SolidColorBrush GetAccentColorBrush(ThemeColorStyle themeColor = ThemeColorStyle.AccentColor)
        {
            var accentColorBrush = Application.Current.TryFindResource($"{themeColor}") as SolidColorBrush;
            if (accentColorBrush != null)
            {
                return accentColorBrush;
            }
            return Orc.Controls.ThemeHelper.GetThemeColorBrush(themeColor);
        }

        #endregion
    }
}
