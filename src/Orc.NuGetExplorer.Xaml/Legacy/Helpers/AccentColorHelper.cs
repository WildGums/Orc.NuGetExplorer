// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccentColorHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows;
    using System.Windows.Media;

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

        private static SolidColorBrush GetAccentColorBrush()
        {
            var accentColorBrush = Application.Current.TryFindResource("AccentColorBrush") as SolidColorBrush;
            if (accentColorBrush != null)
            {
                return accentColorBrush;
            }
            return new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }
        #endregion
    }
}
