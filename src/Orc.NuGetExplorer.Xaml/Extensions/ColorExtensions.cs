// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows;
    using System.Windows.Media;
    using Catel.IoC;
    using Orc.Controls;
    using Orc.Theming;

    internal static class ColorExtensions
    {
        #region Fields
        private static ResourceDictionary AccentColorResourceDictionary;
        #endregion

        #region Methods
        public static ResourceDictionary CreateAccentColorResourceDictionary(this Color color)
        {
            if (AccentColorResourceDictionary != null)
            {
                return AccentColorResourceDictionary;
            }

            var themeManager = ThemeManager.Current;

            var resourceDictionary = new ResourceDictionary();

            resourceDictionary.Add("AccentColor", color);
            resourceDictionary.Add("AccentColorBrush", new SolidColorBrush((Color)resourceDictionary["AccentColor"]));

            //accent colors
            for (int accent = 1; accent <= 5; accent++)
            {
                var brush = themeManager.GetThemeColorBrush((ThemeColorStyle)accent);
                resourceDictionary.Add($"AccentColor{accent}", brush.Color);
                resourceDictionary.Add($"AccentColorBrush{accent}", brush);
            }

            //non-alpha accent color
            var nonAlphaAccentColor3 = AccentColorHelper.ConvertToNonAlphaColor(Colors.White, ThemeColorStyle.AccentColor40);
            var nonAlphaAccentColor3Brush = nonAlphaAccentColor3.ToSolidColorBrush();

            resourceDictionary.Add("AccentColorRGB3", nonAlphaAccentColor3);
            resourceDictionary.Add("AccentColorBrushRGB3", nonAlphaAccentColor3Brush);

            //borders
            for (int border = 6; border <= 11; border++)
            {
                var brush = themeManager.GetThemeColorBrush((ThemeColorStyle)border);
                resourceDictionary.Add($"BorderColor{border}", brush.Color);
                resourceDictionary.Add($"BorderColorBrush{border}", brush);
            }

            var backgroundBrush = themeManager.GetThemeColorBrush(ThemeColorStyle.BackgroundColor);
            resourceDictionary.Add($"BackgroundColor", backgroundBrush.Color);
            resourceDictionary.Add($"BackgroundColorBrush", backgroundBrush);


            var foregroundBrush = themeManager.GetThemeColorBrush(ThemeColorStyle.ForegroundColor);
            resourceDictionary.Add($"ForegroundColor", foregroundBrush.Color);
            resourceDictionary.Add($"ForegroundColorBrush", foregroundBrush);

            var foregroundAlternativeBrush = themeManager.GetThemeColorBrush(ThemeColorStyle.MouseOverForeground);
            resourceDictionary.Add($"ForegroundAlternativeColor", foregroundAlternativeBrush.Color);
            resourceDictionary.Add($"ForegroundAlternativeColorBrush", foregroundAlternativeBrush);


            // Aliases - highlights
            AddAliaseResource("DarkHighlight", "AccentColor3", "AccentColorBrush3");
            AddAliaseResource("Highlight", "AccentColor4", "AccentColorBrush4");

            // Aliases - borders
            AddAliaseResource("BorderLight", "BorderColor3", "BorderColorBrush3");
            AddAliaseResource("BorderMedium", "BorderColor2", "BorderColorBrush2");
            AddAliaseResource("BorderDark", "BorderColor1", "BorderColorBrush1");
            AddAliaseResource("BorderMouseOver", "AccentColor1", "AccentColorBrush1");
            AddAliaseResource("BorderPressed", "AccentColor", "AccentColorBrush");
            AddAliaseResource("BorderChecked", "AccentColor", "AccentColorBrush");
            AddAliaseResource("BorderSelected", "AccentColor", "AccentColorBrush");
            AddAliaseResource("BorderSelectedInactive", "AccentColor2", "AccentColorBrush2");
            AddAliaseResource("BorderDisabled", "BorderColor5", "BorderColorBrush5");

            //Aliases - backgrounds
            AddAliaseResource("BackgroundMouseOver", "AccentColor4", "AccentColorBrush4");
            AddAliaseResource("BackgroundPressed", "AccentColor3", "AccentColorBrush3");
            AddAliaseResource("BackgroundChecked", "AccentColor", "AccentColorBrush");
            AddAliaseResource("BackgroundSelected", "AccentColor3", "AccentColorBrush3");
            AddAliaseResource("BackgroundSelectedInactive", "AccentColor4", "AccentColorBrush4");
            AddAliaseResource("BackgroundDisabled", "AccentColor5", "AccentColorBrush5");

            // Aliases - foregrounds

            AddAliaseResource("ForegroundMouseOver", "ForegroundColor", "ForegroundColorBrush");
            AddAliaseResource("ForegroundPressed", "ForegroundAlternativeColor", "ForegroundAlternativeColorBrush");
            AddAliaseResource("ForegroundChecked", "ForegroundAlternativeColor", "ForegroundAlternativeColorBrush");
            AddAliaseResource("ForegroundSelected", "ForegroundAlternativeColor", "ForegroundAlternativeColorBrush");
            AddAliaseResource("ForegroundSelectedInactive", "ForegroundAlternativeColor", "ForegroundAlternativeColorBrush");
            AddAliaseResource("ForegroundDisabled", "ForegroundColor", "ForegroundColorBrush");

            var application = Application.Current;
            var applicationResources = application.Resources;
            applicationResources.MergedDictionaries.Insert(0, resourceDictionary);

            AccentColorResourceDictionary = resourceDictionary;
            return applicationResources;

            void AddAliaseResource(string colorAlias, string resourceColorKey, string resourceBrushKey)
            {
                resourceDictionary.Add(colorAlias, resourceDictionary[resourceColorKey]);
                resourceDictionary.Add($"{colorAlias}Brush", resourceDictionary[resourceBrushKey]);
            }
        }

        #endregion
    }
}
