using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MetalSelling;

public partial class HistoryPage : UserControl
{
    public HistoryPage()
    {
        InitializeComponent();

        RefreshData();
    }
    private void RefreshData()
    {
        MainListBox.ItemsSource = App.DataBaseContext.Histories.Include("IdProductNavigation").OrderByDescending(x => x.Id).ToList();
    }
}