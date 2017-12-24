// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
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

            if (package.ApiValidations.Count > 0)
            {
                var validations = GetAlertRecords("Errors: ", package.ApiValidations.Select(s => " - " + s).ToArray());
                paragraph.Inlines.AddIfNotNull(validations);
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add("You must update the shell in order to apply this update".ToInline(Brushes.Red));
                paragraph.Inlines.Add(new LineBreak());
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

        private Inline GetAlertRecords(string title, params string[] stringLines)
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

            var inlines = valuableLines.Select(line => line.ToInline(Brushes.Red).Append(new LineBreak())).ToList();
            var inline = title.ToInline().Append(new LineBreak());
            var resultInline = inline.Bold().AppendRange(inlines);
            return resultInline;
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

            var inline = title.ToInline();
            var resultInline = inline.Bold().AppendRange(inlines);
            return resultInline;
        }
        #endregion
    }
}