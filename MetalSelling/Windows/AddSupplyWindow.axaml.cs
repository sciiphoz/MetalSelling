using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using MsBox.Avalonia;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MetalSelling;

public partial class AddSupplyWindow : Window
{
    public AddSupplyWindow()
    {
        InitializeComponent();

        IdSupplierComboBox.ItemsSource = App.DataBaseContext.Suppliers.ToList();
        IdProductComboBox.ItemsSource = App.DataBaseContext.Products.ToList();
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (IdSupplierComboBox.SelectedItem == null || IdProductComboBox.SelectedItem == null || AmountTextBox.Text == string.Empty)
            {
                throw new Exception("All fields must be filled.");
            }
            else
            {
                if (int.TryParse(AmountTextBox.Text, out int amount))
                {
                    if (amount > 0)
                    {
                        var selectedProductId = IdProductComboBox.SelectedIndex + 1;
                        var product = App.DataBaseContext.Products.FirstOrDefault(x => x.Id == selectedProductId);

                        if (product == null)
                        {
                            throw new Exception("Selected product not found.");
                        }

                        var newSupply = new Supply()
                        {
                            Id = App.DataBaseContext.Supplies.Any() ? App.DataBaseContext.Supplies.Max(x => x.Id) + 1 : 1,
                            IdSupplier = IdSupplierComboBox.SelectedIndex + 1,
                            IdProduct = selectedProductId,
                            Amount = amount,
                            Price = Math.Round(product.PricePerPiece.Value * amount, 2),
                            Status = "в ожидании",
                            Date = DateOnly.FromDateTime(DateTime.Now)
                        };

                        product.Amount = (product.Amount ?? 0) + amount;

                        App.DataBaseContext.Products.Update(product);
                        App.DataBaseContext.Supplies.Add(newSupply);
                        App.DataBaseContext.SaveChanges();

                        this.Close();
                    }
                    else
                    {
                        throw new Exception("Amount must be greater than 0.");
                    }
                }
                else
                {
                    throw new Exception("Incorrect amount format.");
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