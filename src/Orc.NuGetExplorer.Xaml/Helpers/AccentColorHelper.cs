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
    using Orc.Theming;

    internal static class AccentColorHelper
    {
        #region Methods

        private static SolidColorBrush GetAccentColorBrush(ThemeColorStyle themeColor = ThemeColorStyle.AccentColor)
        {
            var accentColorBrush = Application.Current.TryFindResource($"{themeColor}") as SolidColorBrush;
            if (accentColorBrush != null)
            {
                return accentColorBrush;
            }

            return ThemeManager.Current.GetThemeColorBrush(themeColor);
        }

        public static Color ConvertToNonAlphaColor(Color backgroundColor, Color accentColor)
        {
            var alphaNormalized = accentColor.A / (double)255;

            //calculate rgb from argb with same color
            var newColorR = (byte)(accentColor.R * alphaNormalized + backgroundColor.R * (1 - alphaNormalized));
            byte newColorG = (byte)(accentColor.G * alphaNormalized + backgroundColor.G * (1 - alphaNormalized));
            var newColorB = (byte)(accentColor.B * alphaNormalized + backgroundColor.B * (1 - alphaNormalized));

            return Color.FromRgb(newColorR, newColorG, newColorB);
        }

        #endregion
    }
}
