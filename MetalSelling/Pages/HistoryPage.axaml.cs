using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using System;
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
            .OrderByDescending(x => x.Id)
            .ToList();

        var historyModels = histories.Select(h => new HistoryModel
        {
            Id = h.Id,
            IdProduct = h.IdProduct,
            Quantity = h.Quantity,
            Date = h.Date,
            IdProductNavigation = h.IdProductNavigation,
            RecordType = h.Quantity < 0 ? "Заказ" : "Поставка"
        }).ToList();

        MainListBox.ItemsSource = historyModels;
    }
}

public class HistoryModel
{
    public int Id { get; set; }
    public int? IdProduct { get; set; }
    public int? Quantity { get; set; }
    public DateOnly? Date { get; set; }
    public Product? IdProductNavigation { get; set; }
    public string RecordType { get; set; } = string.Empty;
}