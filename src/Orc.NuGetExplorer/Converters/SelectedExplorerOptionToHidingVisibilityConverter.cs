// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedExplorerOptionToHidingVisibilityConverter.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class SelectedExplorerOptionToHidingVisibilityConverter : HidingVisibilityConverterBase
    {
        #region Methods
        protected override bool IsVisible(object value, Type targetType, object parameter)
        {
            var stringParameter = parameter as string;
            var stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                return string.Equals(stringValue, stringParameter);
            }

            return false;
        }
        #endregion
    }
}