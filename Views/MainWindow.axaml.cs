using Avalonia.Controls;
using Avalonia.Styling;
using MyFirstAvaloniaApp.ViewModels;

namespace MyFirstAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Подписка на изменение DataContext (когда он установится)
        this.DataContextChanged += (s, e) =>
        {
            if (DataContext is MainViewModel vm)
            {
                // Устанавливаем начальный класс темы
                UpdateThemeClass(vm.IsDarkTheme);
                
                // Подписываемся на изменение IsDarkTheme
                vm.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(MainViewModel.IsDarkTheme))
                    {
                        UpdateThemeClass(vm.IsDarkTheme);
                    }
                };
            }
        };
    }

    private void UpdateThemeClass(bool isDark)
    {
        if (isDark)
        {
            Classes.Remove("LightTheme");
            Classes.Add("DarkTheme");
        }
        else
        {
            Classes.Remove("DarkTheme");
            Classes.Add("LightTheme");
        }
    }
}