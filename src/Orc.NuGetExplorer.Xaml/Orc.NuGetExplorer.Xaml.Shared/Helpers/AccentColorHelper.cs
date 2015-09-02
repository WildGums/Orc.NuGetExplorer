// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccentColorHelper.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows;
    using System.Windows.Media;

    internal class AccentColorHelper
    {
        #region Fields
        private static bool _isAccentColorResourceDictionaryCreated;
        #endregion

        #region Methods
        public static void CreateAccentColorResourceDictionary()
        {
            if (_isAccentColorResourceDictionaryCreated)
            {
                return;
            }

            var accentColor = GetAccentColorBrush().Color;
            accentColor.CreateAccentColorResourceDictionary();

            _isAccentColorResourceDictionaryCreated = true;
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