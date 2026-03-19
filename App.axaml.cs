using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyFirstAvaloniaApp.Services;
using MyFirstAvaloniaApp.ViewModels;
using MyFirstAvaloniaApp.Views;
using System.Threading.Tasks; 

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
            var noteService = new SqliteNoteService();
            
            // Инициализация базы данных (синхронно для простоты)
            Task.Run(async () => await noteService.InitializeDatabaseAsync()).Wait();
            
            var viewModel = new MainViewModel(dialogService, noteService);
            
            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}