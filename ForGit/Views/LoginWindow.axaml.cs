using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ForGit.Services;

namespace ForGit.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private async void Login_Click(object? sender, RoutedEventArgs e)
    {
        var email = EmailBox.Text ?? "";
        var password = PasswordBox.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Введите email и пароль");
            return;
        }

        LoginButton.IsEnabled = false;
        try
        {
            var result = await AuthService.LoginAsync(email, password);
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
            LoginButton.IsEnabled = true;
        }
    }

    private void Register_Click(object? sender, RoutedEventArgs e)
    {
        App.SwitchWindow(this, new RegisterWindow());
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.IsVisible = true;
    }
}
