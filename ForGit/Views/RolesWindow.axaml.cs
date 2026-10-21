using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ForGit.Models;
using ForGit.Services;

namespace ForGit.Views;

public partial class RolesWindow : Window
{
    public RolesWindow()
    {
        InitializeComponent();
        Opened += async (_, _) => await RunAsync(LoadAsync);
    }

    private async Task LoadAsync()
    {
        RolesList.ItemsSource = await RoleService.GetAllAsync();
        ClearSelection();
    }

    private void ClearSelection()
    {
        RolesList.SelectedItem = null;
        NameBox.Text = "";
        DeleteButton.IsEnabled = false;
    }

    private void Role_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var role = RolesList.SelectedItem as Roles;
        NameBox.Text = role?.Name ?? "";
        DeleteButton.IsEnabled = role != null;
    }

    private void New_Click(object? sender, RoutedEventArgs e)
    {
        ClearSelection();
        ErrorText.IsVisible = false;
        NameBox.Focus();
    }

    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        await RunAsync(async () =>
        {
            var error = await RoleService.SaveAsync((RolesList.SelectedItem as Roles)?.Id, NameBox.Text ?? "");
            if (error != null) ShowError(error);
            else await LoadAsync();
        });
    }

    private async void Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (RolesList.SelectedItem is not Roles role) return;
        var confirm = new Window
        {
            Title = "Удаление роли", Width = 350, SizeToContent = SizeToContent.Height,
            CanResize = false, WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        var panel = new StackPanel { Spacing = 8, Margin = new Avalonia.Thickness(24) };
        panel.Children.Add(new TextBlock { Text = $"Удалить роль «{role.Name}»?", TextWrapping = Avalonia.Media.TextWrapping.Wrap });
        var yes = new Button { Content = "Удалить" };
        var no = new Button { Content = "Отмена" };
        yes.Click += (_, _) => confirm.Close(true);
        no.Click += (_, _) => confirm.Close(false);
        panel.Children.Add(yes);
        panel.Children.Add(no);
        confirm.Content = panel;
        if (!await confirm.ShowDialog<bool>(this)) return;
        await RunAsync(async () =>
        {
            var error = await RoleService.DeleteAsync(role.Id);
            if (error != null) ShowError(error);
            else await LoadAsync();
        });
    }

    private async void Refresh_Click(object? sender, RoutedEventArgs e) => await RunAsync(LoadAsync);

    private async Task RunAsync(Func<Task> action)
    {
        EditorPanel.IsEnabled = false;
        RolesList.IsEnabled = false;
        ErrorText.IsVisible = false;
        try { await action(); }
        catch (Exception) { ShowError("Не удалось выполнить операцию. Проверьте подключение к базе данных и обновите список."); }
        finally
        {
            EditorPanel.IsEnabled = true;
            RolesList.IsEnabled = true;
        }
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.IsVisible = true;
    }
}
