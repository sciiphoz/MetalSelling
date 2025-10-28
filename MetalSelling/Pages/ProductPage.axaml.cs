using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        var editWindow = new EditProductWindow(selectedProduct);
        var parent = this.VisualRoot as Window;
        await editWindow.ShowDialog(parent);

        RefreshData();
    }
    private void RefreshData()
    {
        MainListBox.ItemsSource = App.DataBaseContext.Products.Include("IdMetalTypeNavigation").ToList();
    }
}