// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Windows;
    using System.Windows.Documents;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;

    public class PackageDetailsViewModel : ViewModelBase
    {
        #region Constructors
        public PackageDetailsViewModel(PackageDetails package)
        {
            Argument.IsNotNull(() => package);

            Package = package;
            PackageSummary = new FlowDocument();
            Paragraph p = new Paragraph(new Run(Package.Id));
            p.FontSize = 14;
            p.FontStyle = FontStyles.Oblique;

            PackageSummary.Blocks.Add(p);
        }
        #endregion

        #region Properties

        [Model(SupportIEditableObject = false)]
        public PackageDetails Package { get; private set; }

        public FlowDocument PackageSummary { get; private set; }
        #endregion
    }
}