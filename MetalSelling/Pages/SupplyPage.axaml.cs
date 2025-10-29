using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System.Linq;
using System.Collections.Generic;

namespace MetalSelling;

public partial class SupplyPage : UserControl
{
    public SupplyPage()
    {
        InitializeComponent();
        RefreshData();
    }

    private async void AddSupplyButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var addWindow = new AddSupplyWindow();
        var parent = this.VisualRoot as Window;
        await addWindow.ShowDialog(parent);

        RefreshData();
    }

    private async void DeleteSupplyButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Are you sure you want to delete this supply?",
            MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);

        var result = await box.ShowAsync();

        if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
        {
            var selectedSupply = MainListBox.SelectedItem as Supply;

            if (selectedSupply == null) return;

            App.DataBaseContext.Supplies.Remove(selectedSupply);
            App.DataBaseContext.SaveChanges();

            RefreshData();
        }
    }

    private void RefreshData()
    {
        var supplies = App.DataBaseContext.Supplies
            .Include("IdSupplierNavigation")
            .Include("IdProductNavigation")
            .Include("IdProductNavigation.IdMetalTypeNavigation")
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToList();

        var supplyViewModels = supplies.Select(s => new SupplyViewModel
        {
            Id = s.Id,
            IdSupplier = s.IdSupplier,
            IdProduct = s.IdProduct,
            Amount = s.Amount ?? 0,
            Price = s.Price ?? 0,
            Status = s.Status ?? "pending",
            Date = s.Date?.ToString("dd.MM.yyyy") ?? "No date",
            IdSupplierNavigation = s.IdSupplierNavigation,
            IdProductNavigation = s.IdProductNavigation,
            StatusColor = GetStatusColor(s.Status ?? "pending"),
            StatusBackground = GetStatusBackground(s.Status ?? "pending"),
            SupplyType = s.Status?.ToLower() == "automatic" ? "AUTO SUPPLY" : "MANUAL SUPPLY",
            TypeColor = s.Status?.ToLower() == "automatic" ? "#e67e22" : "#3498db"
        }).ToList();

        MainListBox.ItemsSource = supplyViewModels;
    }

    private string GetStatusColor(string status)
    {
        return status.ToLower() switch
        {
            "completed" or "завершена" => "#27ae60",
            "in transit" or "в пути" => "#3498db",
            "pending" or "в ожидании" => "#f39c12",
            "automatic" or "автоматическая" => "#9b59b6",
            _ => "#95a5a6"
        };
    }

    private string GetStatusBackground(string status)
    {
        return status.ToLower() switch
        {
            "completed" or "завершена" => "#27ae60",
            "in transit" or "в пути" => "#3498db",
            "pending" or "в ожидании" => "#f39c12",
            "automatic" or "автоматическая" => "#9b59b6",
            _ => "#95a5a6"
        };
    }
}

public class SupplyViewModel
{
    public int Id { get; set; }
    public int? IdSupplier { get; set; }
    public int? IdProduct { get; set; }
    public int Amount { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public Supplier? IdSupplierNavigation { get; set; }
    public Product? IdProductNavigation { get; set; }
    public string StatusColor { get; set; } = "#95a5a6";
    public string StatusBackground { get; set; } = "#95a5a6";
    public string SupplyType { get; set; } = "MANUAL SUPPLY";
    public string TypeColor { get; set; } = "#3498db";
}