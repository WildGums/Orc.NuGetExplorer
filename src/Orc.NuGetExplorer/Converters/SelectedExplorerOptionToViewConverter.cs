// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedExplorerOptionToViewConverter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.Windows.Data.Converters;
    using Orc.NuGetExplorer.Views;

    public class SelectedExplorerOptionToViewConverter : ValueConverterBase
    {
        #region Methods
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                if (string.Equals(stringValue, "Installed"))
                {
                    return new InstalledExtensionsView();
                }

                if (string.Equals(stringValue, "Online"))
                {
                    return new OnlineExtensionsView();
                }

                if (string.Equals(stringValue, "Updates"))
                {
                    return new UpdateExtensionsView();
                }
            }

            return null;
        }
        #endregion
    }
}