using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MetalSelling;

public partial class HistoryPage : UserControl
{
    public HistoryPage()
    {
        InitializeComponent();
        RefreshData();
    }

    private void RefreshData()
    {
        var histories = App.DataBaseContext.Histories
            .Include("IdProductNavigation")
            .Include("IdProductNavigation.IdMetalTypeNavigation")
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToList();

        var historyViewModels = histories.Select(h => new HistoryViewModel
        {
            Id = h.Id,
            IdProduct = h.IdProduct,
            Quantity = h.Quantity ?? 0,
            Date = h.Date?.ToString("dd.MM.yyyy") ?? "No date",
            IdProductNavigation = h.IdProductNavigation,
            RecordType = h.Quantity < 0 ? "ORDER" : "SUPPLY",
            IsSupply = h.Quantity >= 0,
            IsOrder = h.Quantity < 0,
            RecordTypeColor = h.Quantity < 0 ? "#e74c3c" : "#27ae60",
            QuantityColor = h.Quantity < 0 ? "#e74c3c" : "#27ae60",
            BorderColor = h.Quantity < 0 ? "#e74c3c" : "#27ae60" // Добавляем цвет рамки
        }).ToList();

        MainListBox.ItemsSource = historyViewModels;
    }
}

public class HistoryViewModel
{
    public int Id { get; set; }
    public int? IdProduct { get; set; }
    public int Quantity { get; set; }
    public string Date { get; set; } = string.Empty;
    public Product? IdProductNavigation { get; set; }
    public string RecordType { get; set; } = string.Empty;
    public bool IsSupply { get; set; }
    public bool IsOrder { get; set; }
    public string RecordTypeColor { get; set; } = "#3498db";
    public string QuantityColor { get; set; } = "#3498db";
    public string BorderColor { get; set; } = "#3498db"; // Новое свойство для цвета рамки
}