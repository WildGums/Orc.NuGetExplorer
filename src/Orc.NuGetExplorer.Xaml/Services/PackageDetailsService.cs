// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Catel;
    using Catel.Logging;

    internal class PackageDetailsService : IPackageDetailsService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods
        public async Task<FlowDocument> PackageToFlowDocument(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            var result = new FlowDocument();

            var paragraph = new Paragraph() {FontSize = 12};

            var autors = GetDetailsRecord("Created by: ", package.Authors.ToArray());
            paragraph.Inlines.Add(autors);

            var id = GetDetailsRecord("Id: ", package.Id);
            paragraph.Inlines.Add(id);

            var version = GetDetailsRecord("Version: ", package.Version.ToString());
            paragraph.Inlines.Add(version);

            if (package.Published != null)
            {
                var published = GetDetailsRecord("Published: ", package.Published.Value.ToLocalTime().ToString());
                paragraph.Inlines.Add(published);
            }

            var downloads = GetDetailsRecord("Downloads: ", package.DownloadCount.ToString());
            paragraph.Inlines.Add(downloads);

            if (!string.IsNullOrWhiteSpace(package.Dependencies))
            {
                var dependencies = GetDetailsRecord("Dependencies: ", package.Dependencies);
                paragraph.Inlines.Add(dependencies);
            }

            result.Blocks.Add(paragraph);
            return result;
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