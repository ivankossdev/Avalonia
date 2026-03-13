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
            var mainWindow = new MainWindow();
            var dialogService = new DialogService(mainWindow);
            var viewModel = new MainViewModel(dialogService);
            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            // Для мобильных/браузера аналогично, но там MainView
            // Можно пока оставить как есть или добавить позже
        }

        base.OnFrameworkInitializationCompleted();
    }
}