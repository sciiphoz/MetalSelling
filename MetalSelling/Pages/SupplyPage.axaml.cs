using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System.Linq;

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
        var box = MessageBoxManager.GetMessageBoxStandard("Warning.", "Are you sure you want to cancel this supply?", MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);

        var result = await box.ShowAsync();

        if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
        {
            var selectedSupply = MainListBox.SelectedItem as Supply;

            if (selectedSupply == null) return;

            App.DataBaseContext.Supplies.Remove(selectedSupply);
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
        MainListBox.ItemsSource = App.DataBaseContext.Supplies.Include("IdProductNavigation").Include("IdSupplierNavigation").OrderByDescending(x => x.Id).ToList();
    }
}