using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System.Linq;
using System.Collections.Generic;

namespace MetalSelling;

public partial class ProductPage : UserControl
{
    public ProductPage()
    {
        InitializeComponent();
        RefreshData();
    }

    private async void MainListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var selectedProduct = MainListBox.SelectedItem as Product;

        if (selectedProduct == null) return;

        ContextProduct.Product = selectedProduct;

        var editWindow = new EditProductWindow();
        var parent = this.VisualRoot as Window;
        await editWindow.ShowDialog(parent);

        RefreshData();
    }

    private void RefreshData()
    {
        var products = App.DataBaseContext.Products
            .Include("IdMetalTypeNavigation")
            .OrderByDescending(x => x.Id)
            .ToList();

        var productViewModels = products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Title = p.Title,
            Amount = p.Amount ?? 0,
            PricePerPiece = p.PricePerPiece ?? 0,
            IdMetalType = p.IdMetalType,
            IdMetalTypeNavigation = p.IdMetalTypeNavigation,
            MetalTypeColor = GetMetalTypeColor(p.IdMetalTypeNavigation?.Title),
            StockLevelColor = GetStockLevelColor(p.Amount ?? 0),
            StockLevelBackground = GetStockLevelBackground(p.Amount ?? 0),
            StockLevelText = GetStockLevelText(p.Amount ?? 0),
            LastActivity = "Recently"
        }).ToList();

        MainListBox.ItemsSource = productViewModels;
    }

    private string GetMetalTypeColor(string? metalType)
    {
        return metalType?.ToLower() switch
        {
            "copper" or "медь" => "#e67e22",
            "steel" or "сталь" => "#95a5a6",
            "aluminum" or "алюминий" => "#3498db",
            "brass" or "латунь" => "#f1c40f",
            "bronze" or "бронза" => "#d35400",
            "cast iron" or "чугун" => "#2c3e50",
            "stainless steel" or "нержавейка" => "#7f8c8d",
            _ => "#9b59b6"
        };
    }

    private string GetStockLevelColor(int amount)
    {
        return amount switch
        {
            < 50 => "#e74c3c",
            < 150 => "#f39c12",
            _ => "#27ae60"
        };
    }

    private string GetStockLevelBackground(int amount)
    {
        return amount switch
        {
            < 50 => "#e74c3c",
            < 150 => "#f39c12",
            _ => "#27ae60"
        };
    }

    private string GetStockLevelText(int amount)
    {
        return amount switch
        {
            < 50 => "LOW STOCK",
            < 150 => "MEDIUM",
            _ => "IN STOCK"
        };
    }

    private void TextBlock_ActualThemeVariantChanged(object? sender, System.EventArgs e)
    {
    }
}

public class ProductViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int Amount { get; set; }
    public decimal PricePerPiece { get; set; }
    public int? IdMetalType { get; set; }
    public MetalType? IdMetalTypeNavigation { get; set; }
    public string MetalTypeColor { get; set; } = "#3498db";
    public string StockLevelColor { get; set; } = "#3498db";
    public string StockLevelBackground { get; set; } = "#3498db";
    public string StockLevelText { get; set; } = "IN STOCK";
    public string LastActivity { get; set; } = string.Empty;
}