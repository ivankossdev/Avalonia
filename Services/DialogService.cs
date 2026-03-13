using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace MyFirstAvaloniaApp.Services;

public class DialogService : IDialogService
{
    private readonly Window _mainWindow;

    public DialogService(Window mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(10),
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 10)
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                        {
                            new Button { Content = "Да", Tag = true, Margin = new Thickness(5) },
                            new Button { Content = "Нет", Tag = false, Margin = new Thickness(5) }
                        }
                    }
                }
            }
        };

        // Обработка нажатия кнопок
        foreach (var child in ((StackPanel)((StackPanel)dialog.Content).Children[1]).Children)
        {
            if (child is Button btn)
            {
                btn.Click += (s, e) =>
                {
                    var result = (bool?)btn.Tag; // Используем bool? чтобы избежать предупреждения
                    dialog.Close(result ?? false);
                };
            }
        }

        var result = await dialog.ShowDialog<bool?>(_mainWindow);
        return result ?? false;
    }

    public async Task ShowInfoAsync(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 250,
            Height = 120,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(10),
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 10)
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 60
                    }
                }
            }
        };

        var okButton = (Button)((StackPanel)dialog.Content).Children[1];
        okButton.Click += (s, e) => dialog.Close();

        await dialog.ShowDialog(_mainWindow);
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        await ShowInfoAsync(title, message);
    }
}