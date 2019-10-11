// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceImageConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer.Converters
{
    using System;
    using Catel.IoC;
    using Catel.MVVM.Converters;

    internal class PackageSourceImageConverter : ValueConverterBase<EditablePackageSource>
    {
        #region Fields
        private IImageResolveService _imageResolveService;
        #endregion

        #region Properties
        private IImageResolveService ImageResolveService
        {
            get
            {
                if (_imageResolveService == null)
                {
                    var serviceLocator = this.GetServiceLocator();
                    _imageResolveService = serviceLocator.ResolveType<IImageResolveService>();
                }

                return _imageResolveService;
            }
        }
        #endregion


        protected override object Convert(EditablePackageSource value, Type targetType, object parameter)
        {
            if (value == null)
            {
                return ImageResolveService.ResolveImageFromUri(new Uri(ResourcePaths.PackageDefaultIcon), null);
            }

            return false;
        }
    }
}
