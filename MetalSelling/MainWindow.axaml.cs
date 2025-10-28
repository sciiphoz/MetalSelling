using Avalonia.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using MsBox.Avalonia;

namespace MetalSelling
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContentControl.Content = new ClientPage();
        }

        private async void CreateReportButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var orders = await App.DataBaseContext.Orders
                    .Include(o => o.IdProductNavigation)
                    .Where(o => o.Date >= firstDayOfMonth && o.Date <= lastDayOfMonth)
                    .ToListAsync();

                var supplies = await App.DataBaseContext.Supplies
                    .Include(s => s.IdProductNavigation)
                    .Where(s => s.Date >= firstDayOfMonth && s.Date <= lastDayOfMonth)
                    .ToListAsync();

                var fileName = $"�����_��_{today:MMMM_yyyy}.docx";
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    AddParagraph(body, $"����� �� {today:MMMM yyyy}", true, 24, JustificationValues.Center);
                    AddParagraph(body, $"�����������: {DateTime.Now:dd.MM.yyyy HH:mm}", false, 12, JustificationValues.Center);
                    AddParagraph(body, "", false, 12, JustificationValues.Left);

                    AddParagraph(body, "������", true, 16, JustificationValues.Left);
                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            AddParagraph(body, $"{order.Date:dd.MM.yyyy} - {order.IdProductNavigation?.Title}: {order.Quantity} ��. - {order.Price:C}", false, 12, JustificationValues.Left);
                        }
                        AddParagraph(body, $"����� �������: {orders.Sum(o => o.Quantity)} ��. �� ����� {orders.Sum(o => o.Price):C}", true, 12, JustificationValues.Left);
                    }
                    else
                    {
                        AddParagraph(body, "������� �� ������ ���", false, 12, JustificationValues.Left);
                    }
                    AddParagraph(body, "", false, 12, JustificationValues.Left);

                    AddParagraph(body, "��������", true, 16, JustificationValues.Left);
                    if (supplies.Any())
                    {
                        foreach (var supply in supplies)
                        {
                            AddParagraph(body, $"{supply.Date:dd.MM.yyyy} - {supply.IdProductNavigation?.Title}: {supply.Amount} ��. - {supply.Price:C}", false, 12, JustificationValues.Left);
                        }
                        AddParagraph(body, $"����� ��������: {supplies.Sum(s => s.Amount)} ��. �� ����� {supplies.Sum(s => s.Price):C}", true, 12, JustificationValues.Left);
                    }
                    else
                    {
                        AddParagraph(body, "�������� �� ������ ���", false, 12, JustificationValues.Left);
                    }
                    AddParagraph(body, "", false, 12, JustificationValues.Left);

                    AddParagraph(body, "��������� �� ���������", true, 16, JustificationValues.Left);
                    var allProducts = await App.DataBaseContext.Products.ToListAsync();

                    foreach (var product in allProducts)
                    {
                        var productOrders = orders.Where(o => o.IdProduct == product.Id).Sum(o => o.Quantity);
                        var productSupplies = supplies.Where(s => s.IdProduct == product.Id).Sum(s => s.Amount);
                        var balanceChange = productSupplies - productOrders;
                        var profit = orders.Where(o => o.IdProduct == product.Id).Sum(o => o.Price) -
                                    (supplies.Where(s => s.IdProduct == product.Id).Sum(s => s.Amount) *
                                     (product.PricePerPiece - (product.PricePerPiece / 1.5m))); // ������ ������� �������

                        if (productOrders > 0 || productSupplies > 0)
                        {
                            AddParagraph(body, $"{product.Title}:", true, 12, JustificationValues.Left);
                            AddParagraph(body, $"  �������: {productOrders} ��.", false, 12, JustificationValues.Left);
                            AddParagraph(body, $"  ����������: {productSupplies} ��.", false, 12, JustificationValues.Left);
                            AddParagraph(body, $"  ��������� �������: {balanceChange} ��.", false, 12, JustificationValues.Left);
                            AddParagraph(body, $"  �������: {profit:C}", false, 12, JustificationValues.Left);
                            AddParagraph(body, "", false, 12, JustificationValues.Left);
                        }
                    }

                    AddParagraph(body, "�������� ����������", true, 16, JustificationValues.Left);
                    var totalOrdersQuantity = orders.Sum(o => o.Quantity);
                    var totalOrdersPrice = orders.Sum(o => o.Price);
                    var totalSuppliesQuantity = supplies.Sum(s => s.Amount);
                    var totalSuppliesPrice = supplies.Sum(s => s.Price);
                    var totalBalanceChange = totalSuppliesQuantity - totalOrdersQuantity;
                    var estimatedProfit = totalOrdersPrice - (totalSuppliesPrice * 0.67m); // ��������� ������� (33% �������)

                    AddParagraph(body, $"����� ���������� ������: {totalOrdersQuantity} ��.", false, 12, JustificationValues.Left);
                    AddParagraph(body, $"����� ����� ������: {totalOrdersPrice:C}", false, 12, JustificationValues.Left);
                    AddParagraph(body, $"����� ���������� ��������: {totalSuppliesQuantity} ��.", false, 12, JustificationValues.Left);
                    AddParagraph(body, $"����� ����� ��������: {totalSuppliesPrice:C}", false, 12, JustificationValues.Left);
                    AddParagraph(body, $"��������� ������ �������: {totalBalanceChange} ��.", false, 12, JustificationValues.Left);
                    AddParagraph(body, $"��������� �������: {estimatedProfit:C}", true, 14, JustificationValues.Left);

                    mainPart.Document.Save();
                }

                var box = MessageBoxManager.GetMessageBoxStandard("�����",
                    $"����� ������� ����������� � �������� �:\n{filePath}",
                    MsBox.Avalonia.Enums.ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Success);
                await box.ShowAsync();
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("������",
                    $"������ ��� ������������ ������: {ex.Message}",
                    MsBox.Avalonia.Enums.ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error);
                await box.ShowAsync();
            }
        }

        private void AddParagraph(Body body, string text, bool isBold, int fontSize, JustificationValues alignment)
        {
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Justification = new Justification() { Val = alignment };
            paragraph.AppendChild(paragraphProperties);

            Run run = new Run();
            RunProperties runProperties = new RunProperties();
            if (isBold)
            {
                runProperties.Bold = new Bold();
            }
            runProperties.FontSize = new FontSize() { Val = (fontSize * 2).ToString() }; // FontSize � ��������� ������
            run.RunProperties = runProperties;
            run.AppendChild(new Text(text));

            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }

        private void SuppliesPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new SupplyPage();
        }

        private void OrdersPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new OrderPage();
        }

        private void ClientsPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new ClientPage();
        }

        private void ProductsPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new ProductPage();
        }

        private void HistoriesPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new HistoryPage();
        }
    }
}