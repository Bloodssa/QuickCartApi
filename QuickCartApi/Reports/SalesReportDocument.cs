using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuickCartApi.DTOs.Admin.Reports;
using System.Globalization;

namespace QuickCartApi.Reports;

public class SalesReportDocument(SalesReportResponse report, IReadOnlyList<SalesTransactionRow> rows, bool truncated) : IDocument
{
    private const string Accent = "#ff5e1f";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static string Money(decimal v) => $"PHP {v.ToString("N2", Inv)}";
    private static string Manila(DateTime utc) => utc.AddHours(8).ToString("MMM d, yyyy h:mm tt", Inv);
    private static string Day(DateOnly d) => d.ToString("MMM d, yyyy", Inv);

    public DocumentMetadata GetMetadata() => new() { Title = $"QuickCart Sales Report {report.From:yyyy-MM-dd} to {report.To:yyyy-MM-dd}" };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(32);
            page.DefaultTextStyle(t => t.FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().PaddingVertical(12).Column(col =>
            {
                col.Spacing(16);
                col.Item().Element(ComposeSummary);
                col.Item().Element(ComposeTopProducts);
                col.Item().Element(ComposeTransactions);
            });
            page.Footer().AlignCenter().Text(t =>
            {
                t.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                t.Span("Page ");
                t.CurrentPageNumber();
                t.Span(" of ");
                t.TotalPages();
            });
        });
    }

    private void ComposeHeader(IContainer c) => c.Column(col =>
    {
        col.Item().Text(t =>
        {
            t.Span("Quick").Bold().FontSize(20);
            t.Span("Cart").Bold().FontSize(20).FontColor(Accent);
        });
        col.Item().Text("Sales Report").FontSize(14).SemiBold();
        col.Item().Text($"{Day(report.From)} to {Day(report.To)}").FontColor(Colors.Grey.Darken2);
        col.Item().Text($"Generated {Manila(DateTime.UtcNow)}").FontSize(8).FontColor(Colors.Grey.Darken1);
        col.Item().PaddingTop(6).LineHorizontal(2).LineColor(Accent);
    });

    private void ComposeSummary(IContainer c) => c.Row(row =>
    {
        row.Spacing(8);
        Stat(row.RelativeItem(), "Paid revenue", Money(report.Summary.PaidRevenue));
        Stat(row.RelativeItem(), "Orders", report.Summary.TotalOrders.ToString(Inv));
        Stat(row.RelativeItem(), "Average order", Money(report.Summary.AverageOrderValue));
        Stat(row.RelativeItem(), "Cancelled", report.Summary.CancelledOrders.ToString(Inv));
    });

    private static void Stat(IContainer c, string label, string value) =>
        c.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(col =>
        {
            col.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1);
            col.Item().Text(value).FontSize(13).Bold();
        });

    private void ComposeTopProducts(IContainer c) => c.Column(col =>
    {
        col.Item().PaddingBottom(4).Text("Top products").FontSize(12).SemiBold();

        if (report.TopProducts.Count == 0)
        {
            col.Item().Text("No sales in this period.").FontColor(Colors.Grey.Darken1);
            return;
        }

        col.Item().Table(table =>
        {
            table.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.ConstantColumn(80); });
            table.Header(h =>
            {
                h.Cell().Element(HeaderCell).Text("Product");
                h.Cell().Element(HeaderCell).AlignRight().Text("Units sold");
            });
            foreach (var p in report.TopProducts)
            {
                table.Cell().Element(BodyCell).Text(p.Name);
                table.Cell().Element(BodyCell).AlignRight().Text(p.Quantity.ToString(Inv));
            }
        });
    });

    private void ComposeTransactions(IContainer c) => c.Column(col =>
    {
        col.Item().PaddingBottom(4).Text($"Transactions ({report.TotalResults})").FontSize(12).SemiBold();

        if (truncated)
            col.Item().PaddingBottom(4).Text($"Showing the latest {rows.Count} of {report.TotalResults} transactions.")
                .FontSize(8).FontColor(Colors.Grey.Darken1);

        if (rows.Count == 0)
        {
            col.Item().Text("No transactions in this period.").FontColor(Colors.Grey.Darken1);
            return;
        }

        col.Item().Table(table =>
        {
            table.ColumnsDefinition(cd =>
            {
                cd.ConstantColumn(95);   // date
                cd.RelativeColumn(2);    // order number
                cd.RelativeColumn(1.5f); // customer
                cd.ConstantColumn(48);   // method
                cd.ConstantColumn(55);   // status
                cd.ConstantColumn(75);   // total
            });

            // QuestPDF repeats the header row on every page
            table.Header(h =>
            {
                h.Cell().Element(HeaderCell).Text("Date");
                h.Cell().Element(HeaderCell).Text("Order");
                h.Cell().Element(HeaderCell).Text("Customer");
                h.Cell().Element(HeaderCell).Text("Method");
                h.Cell().Element(HeaderCell).Text("Payment");
                h.Cell().Element(HeaderCell).AlignRight().Text("Total");
            });

            foreach (var r in rows)
            {
                table.Cell().Element(BodyCell).Text(Manila(r.Date)).FontSize(8);
                table.Cell().Element(BodyCell).Text(r.OrderNumber).FontSize(8);
                table.Cell().Element(BodyCell).Text(r.CustomerName).FontSize(8);
                table.Cell().Element(BodyCell).Text(r.PaymentMethod).FontSize(8);
                table.Cell().Element(BodyCell).Text(r.PaymentStatus).FontSize(8);
                table.Cell().Element(BodyCell).AlignRight().Text(Money(r.Total)).FontSize(8);
            }
        });
    });

    private static IContainer HeaderCell(IContainer c) =>
        c.Background(Colors.Grey.Lighten3).Padding(5).DefaultTextStyle(x => x.SemiBold().FontSize(9));

    private static IContainer BodyCell(IContainer c) =>
        c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
}