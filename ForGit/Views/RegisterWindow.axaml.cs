using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ForGit.Services;

namespace ForGit.Views;

public partial class RegisterWindow : Window
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public RegisterWindow()
    {
        InitializeComponent();
    }

    private async void Register_Click(object? sender, RoutedEventArgs e)
    {
        var lastName = LastNameBox.Text ?? "";
        var firstName = FirstNameBox.Text ?? "";
        var middleName = MiddleNameBox.Text ?? "";
        var email = EmailBox.Text ?? "";
        var password = PasswordBox.Text ?? "";
        var confirm = ConfirmPasswordBox.Text ?? "";

        if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
        {
            ShowError("Введите фамилию и имя");
            return;
        }
        if (!EmailRegex.IsMatch(email.Trim()))
        {
            ShowError("Некорректный email");
            return;
        }
        if (password.Length < 6)
        {
            ShowError("Пароль должен содержать не менее 6 символов");
            return;
        }
        if (password != confirm)
        {
            ShowError("Пароли не совпадают");
            return;
        }

        RegisterButton.IsEnabled = false;
        try
        {
            var result = await AuthService.RegisterAsync(firstName, middleName, lastName, email, password);
            if (!result.Success)
            {
                ShowError(result.Error!);
                return;
            }

            App.SwitchWindow(this, new MainWindow(result.User!));
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка подключения к базе данных: {ex.Message}");
        }
        finally
        {
            RegisterButton.IsEnabled = true;
        }
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        App.SwitchWindow(this, new LoginWindow());
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.IsVisible = true;
    }
}
