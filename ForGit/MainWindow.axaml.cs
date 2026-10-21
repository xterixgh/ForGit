using Avalonia.Controls;
using Avalonia.Interactivity;
using ForGit.Models;
using ForGit.Views;

namespace ForGit;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(User user) : this()
    {
        WelcomeText.Text = $"Добро пожаловать, {user.LastName} {user.FirstName} {user.MiddleName}".TrimEnd();
        RoleText.Text = $"Роль: {user.Roles?.Name}";
    }

    private void Logout_Click(object? sender, RoutedEventArgs e)
    {
        App.SwitchWindow(this, new LoginWindow());
    }
}
