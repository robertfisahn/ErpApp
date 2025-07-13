using QuestPDF.Fluent;
using ErpApp.DTO;

namespace ErpApp.Services;

public class ReportPdfGenerator
{
    public byte[] GenerateSalesReport(List<ProductReportDto> data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Header().Text("Sales Report").FontSize(18).Bold();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Product").Bold();
                        header.Cell().Text("Category").Bold();
                        header.Cell().Text("Total Sold").Bold();
                        header.Cell().Text("Total Revenue").Bold();
                    });

                    foreach (var row in data)
                    {
                        table.Cell().Text(row.Name);
                        table.Cell().Text(row.Category);
                        table.Cell().Text(row.TotalSold.ToString());
                        table.Cell().Text($"{row.TotalRevenue:C}");
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated ").Italic();
                    x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                });
            });
        });

        return document.GeneratePdf();
    }
}
