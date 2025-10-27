using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System.Linq;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace MetalSelling;

public partial class ClientPage : UserControl
{
    public ClientPage()
    {
        InitializeComponent();

        RefreshData();
    }

    private async void MainListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var selectedClient = MainListBox.SelectedItem as Client;

        if (selectedClient == null) return;

        ContextClient.Client = selectedClient;

        var editWindow = new EditClientWindow();
        var parent = this.VisualRoot as Window;
        await editWindow.ShowDialog(parent);

        RefreshData();
    }

    private async void AddClientButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var editWindow = new AddClientWindow();
        var parent = this.VisualRoot as Window;
        await editWindow.ShowDialog(parent);

        RefreshData();
    }

    private async void DeleteClientButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Warning.", "Are you sure you want to delete this client?", MsBox.Avalonia.Enums.ButtonEnum.YesNo);

        var result = await box.ShowAsync();

        if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
        {
            var selectedClient = MainListBox.SelectedItem as Client;

            if (selectedClient == null) return;

            App.DataBaseContext.Clients.Remove(selectedClient);
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
        MainListBox.ItemsSource = App.DataBaseContext.Clients.Include("IdClientTypeNavigation").OrderByDescending(x => x.Id).ToList();
    }
}