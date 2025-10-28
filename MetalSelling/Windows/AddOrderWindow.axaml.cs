using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using MsBox.Avalonia;
using System;
using System.Linq;

namespace MetalSelling;

public partial class AddOrderWindow : Window
{
    public AddOrderWindow()
    {
        InitializeComponent();
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (IdProductComboBox.SelectedItem == null || IdClientComboBox.SelectedItem == null || QuantityTextBox.Text == string.Empty)
            {
                throw new Exception("All fields must be filled.");
            }
            else
            {
                if (int.TryParse(QuantityTextBox.Text, out int quantity))
                {
                    var selectedProductId = IdProductComboBox.SelectedIndex + 1;
                    var product = App.DataBaseContext.Products.FirstOrDefault(x => x.Id == selectedProductId);

                    if (product == null)
                    {
                        throw new Exception("Selected product not found.");
                    }

                    if (quantity <= 0 || quantity > product.Amount)
                    {
                        throw new Exception($"Incorrect quantity amount. Available: {product.Amount}");
                    }

                    var newOrder = new Order()
                    {
                        Id = App.DataBaseContext.Orders.Any() ? App.DataBaseContext.Orders.Max(x => x.Id) + 1 : 1,
                        IdClient = IdClientComboBox.SelectedIndex + 1,
                        IdProduct = selectedProductId,
                        Quantity = quantity,
                        Price = Math.Round(product.PricePerPiece.Value * quantity, 2),
                        Status = "в ожидании",
                        Date = DateOnly.FromDateTime(DateTime.Now)
                    };

                    product.Amount -= quantity;

                    int minimumStockLevel = 150;
                    int restockAmount = 500;

                    if (product.Amount < minimumStockLevel)
                    {
                        var newSupply = new Supply()
                        {
                            Id = App.DataBaseContext.Supplies.Any() ? App.DataBaseContext.Supplies.Max(x => x.Id) + 1 : 1,
                            IdSupplier = 1,
                            IdProduct = selectedProductId,
                            Amount = restockAmount,
                            Price = Math.Round(product.PricePerPiece.Value * restockAmount, 2),
                            Status = "автоматическая",
                            Date = DateOnly.FromDateTime(DateTime.Now)
                        };

                        product.Amount += restockAmount;

                        App.DataBaseContext.Supplies.Add(newSupply);
                    }

                    App.DataBaseContext.Products.Update(product);
                    App.DataBaseContext.Orders.Add(newOrder);
                    App.DataBaseContext.SaveChanges();

                    this.Close();
                }
                else
                {
                    throw new Exception("Incorrect quantity amount.");
                }
            }
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