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
    using NuGet;

    internal class PackageDetailsService : IPackageDetailsService
    {
        #region Methods
        public async Task<FlowDocument> PackageToFlowDocument(IPackage package)
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

            var dataServicePackage = package as DataServicePackage;
            if (dataServicePackage != null)
            {
                if (dataServicePackage.Published != null)
                {
                    var published = GetDetailsRecord("Published: ", dataServicePackage.Published.Value.ToLocalTime().ToString());
                    paragraph.Inlines.Add(published);
                }

                var downloads = GetDetailsRecord("Downloads: ", dataServicePackage.DownloadCount.ToString());
                paragraph.Inlines.Add(downloads);

                var dependencies = GetDetailsRecord("Dependencies: ", dataServicePackage.Dependencies);
                paragraph.Inlines.Add(dependencies);
            }

            var description = GetDetailsRecord("Description: ", package.Description);
            paragraph.Inlines.Add(description);

            result.Blocks.Add(paragraph);
            return result;
        }

        private Inline GetDetailsRecord(string title, params string[] stringLines)
        {
            Argument.IsNotNullOrWhitespace(() => title);

            if (stringLines == null || !stringLines.Any())
            {
                return null;
            }

            var inlines = stringLines.Select(line => line.ToInline().Append(new LineBreak())).ToList();

            var resultInline = title.ToInline().Bold().AppendRange(inlines);
            return resultInline;
        }
        #endregion
    }
}