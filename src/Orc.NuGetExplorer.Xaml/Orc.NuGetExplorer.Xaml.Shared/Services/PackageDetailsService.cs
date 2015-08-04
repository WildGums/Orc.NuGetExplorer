// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Globalization;
    using System.Linq;
    using System.Windows.Documents;
    using Catel;
    using Catel.Logging;

    internal class PackageDetailsService : IPackageDetailsService
    {
        private readonly IRepositoryNavigatorService _repositoryNavigatorService;

        public PackageDetailsService(IRepositoryNavigatorService repositoryNavigatorService)
        {
            Argument.IsNotNull(() => repositoryNavigatorService);

            _repositoryNavigatorService = repositoryNavigatorService;
        }

        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods
        public FlowDocument PackageToFlowDocument(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            var result = new FlowDocument();

            var paragraph = new Paragraph
            {
                FontSize = 12
            };

            var autors = GetDetailsRecord("Created by: ", package.Authors.ToArray());
            paragraph.Inlines.AddIfNotNull(autors);

            var id = GetDetailsRecord("Id: ", package.Id);
            paragraph.Inlines.AddIfNotNull(id);

            var version = GetDetailsRecord("Version: ", GetVersion(package));
            paragraph.Inlines.AddIfNotNull(version);

            var published = package.Published;

            if (published != null && _repositoryNavigatorService.Navigator.SelectedRepository.OperationType != PackageOperationType.Uninstall)
            {
                paragraph.Inlines.AddIfNotNull(GetDetailsRecord("Published: ", published.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat)));
            }

            if (published != null && _repositoryNavigatorService.Navigator.SelectedRepository.OperationType == PackageOperationType.Uninstall)
            {
                paragraph.Inlines.AddIfNotNull(GetDetailsRecord("Installed: ", published.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat)));
            }

            var downloads = GetDetailsRecord("Downloads: ", package.DownloadCount.ToString());
            paragraph.Inlines.AddIfNotNull(downloads);

            if (!string.IsNullOrWhiteSpace(package.Dependencies))
            {
                var dependencies = GetDetailsRecord("Dependencies: ", package.Dependencies);
                paragraph.Inlines.AddIfNotNull(dependencies);
            }

            result.Blocks.Add(paragraph);
            return result;
        }

        private static string GetVersion(IPackageDetails package)
        {
            if (!package.IsPrerelease)
            {
                return package.Version.ToString();
            }

            return string.Format("{0}-{1}", package.Version, package.SpecialVersion);
        }

        private Inline GetDetailsRecord(string title, params string[] stringLines)
        {
            Argument.IsNotNullOrWhitespace(() => title);

            if (stringLines == null)
            {
                return null;
            }

            var valuableLines = stringLines.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (!valuableLines.Any())
            {
                return null;
            }

            var inlines = valuableLines.Select(line => line.ToInline().Append(new LineBreak())).ToList();

            var resultInline = title.ToInline().Bold().AppendRange(inlines);
            return resultInline;
        }
        #endregion
    }
}