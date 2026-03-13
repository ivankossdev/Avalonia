using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyFirstAvaloniaApp.Services;
using MyFirstAvaloniaApp.ViewModels;
using MyFirstAvaloniaApp.Views;

namespace MyFirstAvaloniaApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Создаём главное окно
            var mainWindow = new MainWindow();
            
            // Создаём сервис диалогов (передаём окно)
            var dialogService = new DialogService(mainWindow);
            
            // Создаём ViewModel – теперь только с DialogService (ThemeService больше не нужен)
            var viewModel = new MainViewModel(dialogService);
            
            // Устанавливаем DataContext для окна
            mainWindow.DataContext = viewModel;
            
            // Назначаем главное окно приложения
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}