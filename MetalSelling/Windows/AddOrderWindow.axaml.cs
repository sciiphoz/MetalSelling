using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MetalSelling;

public partial class AddOrderWindow : Window
{
    private ObservableCollection<OrderItem> _orderItems;

    public AddOrderWindow()
    {
        InitializeComponent();

        _orderItems = new ObservableCollection<OrderItem>();
        OrderItemsListBox.ItemsSource = _orderItems;

        IdClientComboBox.ItemsSource = App.DataBaseContext.Clients.ToList();
        IdProductComboBox.ItemsSource = App.DataBaseContext.Products.ToList();

        UpdateTotalPrice();
    }

    public class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Math.Round((Product.PricePerPiece ?? 0) * Quantity, 2);
    }

    private void AddProductButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (IdProductComboBox.SelectedItem is not Product selectedProduct)
            {
                throw new Exception("Выберите товар.");
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                throw new Exception("Введите корректное количество.");
            }

            if (quantity > selectedProduct.Amount)
            {
                throw new Exception($"Недостаточно товара на складе. Доступно: {selectedProduct.Amount}");
            }

            var existingItem = _orderItems.FirstOrDefault(item => item.Product.Id == selectedProduct.Id);
            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + quantity;
                if (newQuantity > selectedProduct.Amount)
                {
                    throw new Exception($"Общее количество превышает доступное. Доступно: {selectedProduct.Amount}");
                }
                existingItem.Quantity = newQuantity;
            }
            else
            {
                _orderItems.Add(new OrderItem
                {
                    Product = selectedProduct,
                    Quantity = quantity
                });
            }

            QuantityTextBox.Text = "";
            IdProductComboBox.SelectedItem = null;

            UpdateTotalPrice();
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message,
                MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            box.ShowWindowDialogAsync(this);
        }
    }

    private void RemoveProductButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is OrderItem item)
        {
            _orderItems.Remove(item);
            UpdateTotalPrice();
        }
    }

    private void UpdateTotalPrice()
    {
        var total = _orderItems.Sum(item => item.TotalPrice);
        TotalPriceText.Text = $"{total:N2} руб.";
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (IdClientComboBox.SelectedItem == null)
            {
                throw new Exception("Выберите клиента.");
            }

            if (!_orderItems.Any())
            {
                throw new Exception("Добавьте хотя бы один товар в заказ.");
            }

            var selectedClient = (Client)IdClientComboBox.SelectedItem;
            var orderDate = DateOnly.FromDateTime(DateTime.Now);

            foreach (var orderItem in _orderItems)
            {
                var product = orderItem.Product;
                var quantity = orderItem.Quantity;

                var newOrder = new Order()
                {
                    Id = App.DataBaseContext.Orders.Any() ? App.DataBaseContext.Orders.Max(x => x.Id) + 1 : 1,
                    IdClient = selectedClient.Id,
                    IdProduct = product.Id,
                    Quantity = quantity,
                    Price = orderItem.TotalPrice,
                    Status = "в ожидании",
                    Date = orderDate
                };

                product.Amount -= quantity;

                int minimumStockLevel = 150;
                int restockAmount = 500;

                if (product.Amount < minimumStockLevel)
                {
                    var newSupply = new Supply()
                    {
                        Id = App.DataBaseContext.Supplies.Any() ? App.DataBaseContext.Supplies.Max(x => x.Id) + 1 : 1,
                        IdSupplier = 3, 
                        IdProduct = product.Id,
                        Amount = restockAmount,
                        Price = Math.Round((product.PricePerPiece ?? 0) * restockAmount * 0.67m, 2),
                        Status = "автоматическая",
                        Date = DateOnly.FromDateTime(DateTime.Now)
                    };

                    product.Amount += restockAmount;
                    App.DataBaseContext.Supplies.Add(newSupply);
                }

                App.DataBaseContext.Orders.Add(newOrder);
                App.DataBaseContext.Products.Update(product);
            }

            App.DataBaseContext.SaveChanges();

            var successBox = MessageBoxManager.GetMessageBoxStandard("Успех",
                "Заказ успешно создан!",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Success);
            successBox.ShowWindowDialogAsync(this);

            this.Close();
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message,
                MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            box.ShowWindowDialogAsync(this);
        }
    }

    private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}