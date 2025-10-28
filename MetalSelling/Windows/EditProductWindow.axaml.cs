using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using MsBox.Avalonia;
using System;
using System.Linq;

namespace MetalSelling;

public partial class EditProductWindow : Window
{
    private Product _product;
    private Product _currentProduct;
    public EditProductWindow()
    {
        InitializeComponent();

        _product = ContextProduct.Product;
    }

    public EditProductWindow(Product product)
    {
        InitializeComponent();

        product = ContextProduct.Product;
        _currentProduct = product;

        IdMetalTypeComboBox.ItemsSource = App.DataBaseContext.MetalTypes.ToList();

        PricePerPieceTextBox.Text = product.PricePerPiece.ToString();
        TitleTextBox.Text = product.Title;

        IdMetalTypeComboBox.SelectedItem = product.IdMetalTypeNavigation;
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (IdMetalTypeComboBox.SelectedItem == null || string.IsNullOrEmpty(TitleTextBox.Text) || string.IsNullOrEmpty(PricePerPieceTextBox.Text))
            {
                throw new Exception("All fields must be filled.");
            }

            if (!decimal.TryParse(PricePerPieceTextBox.Text, out decimal pricePerPiece))
            {
                throw new Exception("Incorrect price format.");
            }

            if (pricePerPiece <= 0)
            {
                throw new Exception("Price must be greater than 0.");
            }

            pricePerPiece = Math.Round(pricePerPiece, 2);

            _currentProduct.IdMetalType = IdMetalTypeComboBox.SelectedIndex + 1; // или другой способ получения ID типа металла
            _currentProduct.Title = TitleTextBox.Text;
            _currentProduct.PricePerPiece = pricePerPiece;

            App.DataBaseContext.Products.Update(_currentProduct);
            App.DataBaseContext.SaveChanges();

            this.Close();
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error.", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            var parent = this.VisualRoot as Window;
            box.ShowWindowDialogAsync(parent);
        }
    }

    private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}