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
            if (NameTextBox.Text == string.Empty || IdClientTypeComboBox.SelectedItem == null || PhoneTextBox.Text == string.Empty || EmailTextBox.Text == string.Empty)
            {
                throw new Exception("All fields must be filled.");
            }
            else
            {
                if (nameCheck.IsMatch(NameTextBox.Text))
                {
                    if (emailCheck.IsMatch(EmailTextBox.Text))
                    {
                        if (phoneCheck.IsMatch(PhoneTextBox.Text))
                        {
                            _currentClient.Name = NameTextBox.Text;
                            _currentClient.Email = PhoneTextBox.Text;
                            _currentClient.Phone = EmailTextBox.Text;
                            _currentClient.IdClientType = IdClientTypeComboBox.SelectedIndex + 1;

                            App.DataBaseContext.Update(_currentClient);
                            App.DataBaseContext.SaveChanges();

                            this.Close();
                        }
                        else
                        {
                            throw new Exception("Incorrect phone format.");
                        }
                    }
                    else
                    {
                        throw new Exception("Incorrect email format.");
                    }
                }
                else
                {
                    throw new Exception("Full name should consist of surname, name and patronymic.");
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