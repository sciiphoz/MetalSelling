using Avalonia.Controls;

namespace MetalSelling
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContentControl.Content = new ClientPage();
        }

        private void CreateReportButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

        }

        private void SuppliesPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new SupplyPage();
        }

        private void OrdersPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new OrderPage();
        }

        private void ClientsPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContentControl.Content = new ClientPage();
        }
    }
}