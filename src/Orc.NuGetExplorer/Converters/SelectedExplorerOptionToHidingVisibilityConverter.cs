// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedExplorerOptionToViewConverter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.Windows.Data.Converters;

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