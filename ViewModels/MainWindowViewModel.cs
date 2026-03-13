using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyFirstAvaloniaApp.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyFirstAvaloniaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private string _logText = "История нажатий:\n";

    [ObservableProperty]
    private string _loadedData = "Данные ещё не загружены";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isDarkTheme; // true - тёмная тема, false - светлая

    public IAsyncRelayCommand LoadDataCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand ToggleThemeCommand { get; }

    public MainViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync, CanLoadData);
        CancelCommand = new RelayCommand(CancelLoad, () => IsLoading);
        ToggleThemeCommand = new RelayCommand(ToggleTheme);

        // Начальная тема (можно прочитать из настроек, но пока false)
        _isDarkTheme = false;
    }

    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
    }

    [RelayCommand]
    private void Button1() => LogText += "Нажата кнопка 1\n";

    [RelayCommand]
    private void Button2() => LogText += "Нажата кнопка 2\n";

    [RelayCommand]
    private async Task Clear()
    {
        var confirmed = await _dialogService.ShowConfirmationAsync("Подтверждение", "Очистить лог?");
        if (confirmed)
        {
            LogText = "История нажатий:\n";
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            LoadDataCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();

            _cts = new CancellationTokenSource();

            await Task.Delay(3000, _cts.Token);

            if (!_cts.Token.IsCancellationRequested)
            {
                LoadedData = $"Данные загружены: {DateTime.Now:T}";
                await _dialogService.ShowInfoAsync("Успех", "Данные успешно загружены");
            }
        }
        catch (OperationCanceledException)
        {
            LoadedData = "Загрузка отменена";
            await _dialogService.ShowInfoAsync("Информация", "Загрузка была отменена");
        }
        catch (Exception ex)
        {
            LoadedData = $"Ошибка: {ex.Message}";
            await _dialogService.ShowErrorAsync("Ошибка", ex.Message);
        }
        finally
        {
            IsLoading = false;
            _cts?.Dispose();
            _cts = null;
            LoadDataCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanLoadData() => !IsLoading;

    private void CancelLoad() => _cts?.Cancel();
}