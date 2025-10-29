using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetalSelling;

public partial class OrderPage : UserControl
{
    public OrderPage()
    {
        InitializeComponent();
        RefreshData();
    }

    private async void AddOrderButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var addWindow = new AddOrderWindow();
        var parent = this.VisualRoot as Window;
        await addWindow.ShowDialog(parent);
        RefreshData();
    }

    private async void DeleteOrderButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Are you sure you want to cancel this order?",
            MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);

        var result = await box.ShowAsync();

        if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
        {
            if (MainListBox.SelectedItem is OrderViewModel selectedOrderViewModel)
            {
                var ordersToDelete = App.DataBaseContext.Orders
                    .Where(o => o.Date == selectedOrderViewModel.OrderDate && o.IdClient == selectedOrderViewModel.ClientId)
                    .ToList();

                App.DataBaseContext.Orders.RemoveRange(ordersToDelete);
                App.DataBaseContext.SaveChanges();
                RefreshData();
            }
        }
    }

    private void RefreshData()
    {
        var orders = App.DataBaseContext.Orders
            .Include(o => o.IdProductNavigation)
            .Include(o => o.IdClientNavigation)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToList();

        var groupedOrders = orders
            .GroupBy(o => new { o.Date, o.IdClient })
            .Select(g => new OrderViewModel
            {
                OrderDate = g.Key.Date ?? DateOnly.MinValue,
                ClientId = g.Key.IdClient ?? 0,
                ClientName = g.First().IdClientNavigation?.Name ?? "Unknown Client",
                Status = g.First().Status ?? "pending",
                StatusColor = GetStatusColor(g.First().Status ?? "pending"),
                StatusBackground = GetStatusBackground(g.First().Status ?? "pending"),
                OrderItems = g.Select(order => new OrderItemViewModel
                {
                    ProductTitle = order.IdProductNavigation?.Title ?? "Unknown Product",
                    Quantity = order.Quantity ?? 0,
                    ItemPrice = order.Price ?? 0
                }).ToList(),
                TotalPrice = g.Sum(order => order.Price) ?? 0,
                TotalQuantity = g.Sum(order => order.Quantity) ?? 0,
                ProductsCount = g.Count()
            })
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.ClientId)
            .ToList();

        MainListBox.ItemsSource = groupedOrders;
    }

    private string GetStatusColor(string status)
    {
        return status.ToLower() switch
        {
            "completed" or "выполнен" => "#27ae60",
            "pending" or "в ожидании" => "#f39c12",
            "processing" or "в обработке" => "#3498db",
            _ => "#95a5a6"
        };
    }

    private string GetStatusBackground(string status)
    {
        return status.ToLower() switch
        {
            "completed" or "выполнен" => "#27ae60",
            "pending" or "в ожидании" => "#f39c12",
            "processing" or "в обработке" => "#3498db",
            _ => "#95a5a6"
        };
    }
}

public class OrderViewModel
{
    public DateOnly OrderDate { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "#95a5a6";
    public string StatusBackground { get; set; } = "#95a5a6";
    public List<OrderItemViewModel> OrderItems { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
    public int ProductsCount { get; set; }
}

public class OrderItemViewModel
{
    public string ProductTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal ItemPrice { get; set; }
}