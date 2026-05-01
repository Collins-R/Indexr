using System;
using System.Collections.Generic;
using System.Text;
using Indexr.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Indexr.Core.Services
{
    public class DocumentGeneratorService
    {
        /// <summary>
        /// Generates a document index and saves it to the specified output path
        /// </summary>
        public void Generate(Project project, List<IndexGroup> groups,
            List<IndexInclusionGroup> inclusionGroups, string outputPath)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader(project));
                    page.Content().Element(ComposeContent(groups, inclusionGroups));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            })
            .GeneratePdf(outputPath);
        }

        /// <summary>
        /// Composes the document header with project details
        /// </summary>
        private Action<IContainer> ComposeHeader(Project project)
        {
            return container =>
            {
                container.Column(col =>
                {
                    col.Item().Text(project.Name)
                        .FontSize(18).Bold();

                    col.Item().Text($"Version: {project.Version}")
                        .FontSize(11);

                    col.Item().Text($"Date: {DateTime.Now:yyyy/MM/dd}")
                        .FontSize(11);

                    col.Item().PaddingTop(10).LineHorizontal(1);
                });
            };
        }

        /// <summary>
        /// Composes the main content table with all index groups
        /// and inclusion groups in folder order
        /// </summary>
        private Action<IContainer> ComposeContent(List<IndexGroup> groups,
            List<IndexInclusionGroup> inclusionGroups)
        {
            return container =>
            {
                container.PaddingTop(20).Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Section
                        columns.RelativeColumn(4); // Sub-section
                        columns.RelativeColumn(2); // Last Modified
                        columns.RelativeColumn(3); // Location
                    });

                    // Header row
                    table.Header(header =>
                    {
                        header.Cell().Background("#D3D3D3").Padding(5)
                            .Text("Section").Bold();
                        header.Cell().Background("#D3D3D3").Padding(5)
                            .Text("Sub-section").Bold();
                        header.Cell().Background("#D3D3D3").Padding(5)
                            .Text("Last Modified").Bold();
                        header.Cell().Background("#D3D3D3").Padding(5)
                            .Text("Location").Bold();
                    });

                    // Merge and sort all groups by folder path
                    var allRows = BuildSortedRows(groups, inclusionGroups);

                    foreach (var row in allRows)
                    {
                        if (row is IndexGroup group)
                        {
                            var isFirst = true;
                            foreach (var entry in group.Entries)
                            {
                                // Section column — only show on first entry
                                table.Cell().BorderBottom(1).Padding(5)
                                    .Text(isFirst ? group.FolderName : "");

                                // Sub-section
                                table.Cell().BorderBottom(1).Padding(5)
                                    .Text(entry.FileName);

                                // Last Modified
                                table.Cell().BorderBottom(1).Padding(5)
                                    .Text(entry.LastModified.ToString("yyyy/MM/dd"));

                                // Location
                                table.Cell().BorderBottom(1).Padding(5)
                                    .Hyperlink(entry.UncPath ?? entry.FilePath)
                                    .Text("Click here for the file")
                                    .FontColor("#0000FF").Underline();

                                isFirst = false;
                            }
                        }
                        else if (row is IndexInclusionGroup inclusionGroup)
                        {
                            // Section
                            table.Cell().BorderBottom(1).Padding(5)
                                .Hyperlink(inclusionGroup.UncFolderPath ??
                                    inclusionGroup.FolderPath)
                                .Text(inclusionGroup.DisplayName)
                                .FontColor("#0000FF").Underline();

                            // Sub-section — empty
                            table.Cell().BorderBottom(1).Padding(5).Text("");

                            // Last Modified — empty
                            table.Cell().BorderBottom(1).Padding(5).Text("");

                            // Location — empty
                            table.Cell().BorderBottom(1).Padding(5).Text("");
                        }
                    }
                });
            };
        }

        /// <summary>
        /// Merges and sorts index groups and inclusion groups 
        /// by their folder path to maintain natural folder order
        /// </summary>
        private List<object> BuildSortedRows(List<IndexGroup> groups,
            List<IndexInclusionGroup> inclusionGroups)
        {
            var combined = new List<(string SortPath, object Row)>();

            foreach (var group in groups)
                combined.Add((group.FolderPath, group));

            foreach (var inclusion in inclusionGroups)
                combined.Add((inclusion.FolderPath, inclusion));

            return combined
                .OrderBy(x => x.SortPath)
                .Select(x => x.Row)
                .ToList();
        }
    }
}
