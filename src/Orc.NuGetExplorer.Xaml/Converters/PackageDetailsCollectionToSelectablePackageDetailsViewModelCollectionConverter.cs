// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsCollectionToSelectablePackageDetailsViewModelCollectionConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    using Catel.Collections;
    using Catel.MVVM.Converters;

    using Orc.NuGetExplorer.ViewModels;

    public class PackageDetailsCollectionToSelectablePackageDetailsViewModelCollectionConverter : ValueConverterBase<ObservableCollection<IPackageDetails>, ObservableCollection<SelectablePackageDetailsViewModel>>
    {
        #region Methods
        protected override object Convert(ObservableCollection<IPackageDetails> value, Type targetType, object parameter)
        {
            var selectablePackageDetailsViewModelCollection = new ObservableCollection<SelectablePackageDetailsViewModel>();
            value.CollectionChanged += (sender, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Reset)
                    {
                        selectablePackageDetailsViewModelCollection.Clear();
                        selectablePackageDetailsViewModelCollection.AddRange(value.Select(packageDetails => new SelectablePackageDetailsViewModel(packageDetails)));
                    }
                    else if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (IPackageDetails newItem in args.NewItems)
                        {
                            selectablePackageDetailsViewModelCollection.Add(new SelectablePackageDetailsViewModel(newItem));
                        }
                    }
                };

            return selectablePackageDetailsViewModelCollection;
        }
        #endregion
    }
}
