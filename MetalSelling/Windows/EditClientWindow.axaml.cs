using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using MsBox.Avalonia;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MetalSelling;

public partial class EditClientWindow : Window
{
    private Client _client;
    private Client _currentClient;
    public EditClientWindow()
    {
        InitializeComponent();

        _client = ContextClient.Client;
    }

    public EditClientWindow(Client client)
    {
        InitializeComponent();

        client = ContextClient.Client;
        _currentClient = client;

        IdClientTypeComboBox.ItemsSource = App.DataBaseContext.ClientTypes.ToList();
        
        NameTextBox.Text = client.Name;
        PhoneTextBox.Text = client.Phone;
        EmailTextBox.Text = client.Email;

        IdClientTypeComboBox.SelectedItem = client.IdClientTypeNavigation;
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Regex nameCheck = new Regex("[?!,.à-ÿÀ-ß¸¨0-9\\s]");
        Regex emailCheck = new Regex("[a-zA-Z1-9\\-\\._]+@[a-z1-9]+(.[a-z1-9]+){1,}");
        Regex phoneCheck = new Regex("\\+?\\d+([\\(\\s\\-]?\\d+[\\)\\s\\-]?[\\d\\s\\-]+)?");

        try
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) ||
                IdClientTypeComboBox.SelectedItem == null ||
                string.IsNullOrEmpty(PhoneTextBox.Text) ||
                string.IsNullOrEmpty(EmailTextBox.Text))
            {
                throw new Exception("All fields must be filled.");
            }

            if (!nameCheck.IsMatch(NameTextBox.Text))
            {
                throw new Exception("Full name should consist of surname, name and patronymic.");
            }

            if (!emailCheck.IsMatch(EmailTextBox.Text))
            {
                throw new Exception("Incorrect email format.");
            }

            if (!phoneCheck.IsMatch(PhoneTextBox.Text))
            {
                throw new Exception("Incorrect phone format.");
            }

            decimal? personalDiscount = null;
            if (!string.IsNullOrEmpty(DiscountTextBox.Text))
            {
                if (decimal.TryParse(DiscountTextBox.Text, out decimal discount))
                {
                    if (discount < 0 || discount > 100)
                    {
                        throw new Exception("Discount must be between 0 and 100 percent.");
                    }
                    personalDiscount = discount;
                }
                else
                {
                    throw new Exception("Discount must be a valid number.");
                }
            }

            _currentClient.Name = NameTextBox.Text.Trim();
            _currentClient.Email = EmailTextBox.Text.Trim();
            _currentClient.Phone = PhoneTextBox.Text.Trim();
            _currentClient.IdClientType = ((ClientType)IdClientTypeComboBox.SelectedItem).Id;
            _currentClient.PersonalDiscount = personalDiscount;

            App.DataBaseContext.Clients.Update(_currentClient);
            App.DataBaseContext.SaveChanges();

            var successBox = MessageBoxManager.GetMessageBoxStandard("Success",
                "Client updated successfully!",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Success);
            successBox.ShowWindowDialogAsync(this);

            this.Close();
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", ex.Message,
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            box.ShowWindowDialogAsync(this);
        }
    }

    private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}