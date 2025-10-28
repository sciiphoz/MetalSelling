using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetalSelling.Data;
using MsBox.Avalonia;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MetalSelling;

public partial class AddClientWindow : Window
{
    public AddClientWindow()
    {
        InitializeComponent();

        IdClientTypeComboBox.ItemsSource = App.DataBaseContext.ClientTypes.ToList();
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
                            var newClient = new Client()
                            {
                                Id = App.DataBaseContext.Clients.Any() ? Convert.ToInt32(App.DataBaseContext.Clients.Max(x => x.Id).ToString()) + 1 : 1,
                                IdClientType = IdClientTypeComboBox.SelectedIndex + 1,
                                Name = NameTextBox.Text,
                                Email = EmailTextBox.Text,
                                Phone = PhoneTextBox.Text,
                            };

                            App.DataBaseContext.Clients.Add(newClient);
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