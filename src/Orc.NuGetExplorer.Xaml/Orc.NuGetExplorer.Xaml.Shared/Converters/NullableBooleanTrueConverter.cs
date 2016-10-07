// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullableBooleanTrueConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class NullableBooleanTrueConverter : ValueConverterBase<bool?>
    {
        protected override object Convert(bool? value, Type targetType, object parameter)
        {
            return value ?? true;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter)
        {
            return (bool?)value;
        }
    }
}