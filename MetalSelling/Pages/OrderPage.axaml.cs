using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
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
        var box = MessageBoxManager.GetMessageBoxStandard("Warning.", "Are you sure you want to cancel this order?", MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);

        var result = await box.ShowAsync();

        if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
        {
            var selectedOrder = MainListBox.SelectedItem as Order;

            if (selectedOrder == null) return;

            App.DataBaseContext.Orders.Remove(selectedOrder);
            App.DataBaseContext.SaveChanges();

            RefreshData();
        }
        else
        {
            return;
        }
    }

    private void RefreshData()
    {
        MainListBox.ItemsSource = App.DataBaseContext.Orders.Include("IdProductNavigation").Include("IdClientNavigation").OrderByDescending(x => x.Id).ToList();
    }
}