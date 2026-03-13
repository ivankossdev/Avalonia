using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyFirstAvaloniaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Существующий код для лога кнопок
    [ObservableProperty]
    private string _logText = "История нажатий:\n";

    [RelayCommand]
    private void Button1() => LogText += "Нажата кнопка 1\n";

    [RelayCommand]
    private void Button2() => LogText += "Нажата кнопка 2\n";

    [RelayCommand]
    private void Clear() => LogText = "История нажатий:\n";

    // Новые поля для асинхронного примера
    [ObservableProperty]
    private string _loadedData = "Данные ещё не загружены";

    [ObservableProperty]
    private bool _isLoading;

    // CancellationTokenSource для возможности отмены
    private CancellationTokenSource? _cts;

    // Асинхронная команда загрузки
    public IAsyncRelayCommand LoadDataCommand { get; }

    // Команда отмены
    public IRelayCommand CancelCommand { get; }

    public MainViewModel()
    {
        // Инициализация команд
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync, CanLoadData);
        CancelCommand = new RelayCommand(CancelLoad, () => IsLoading);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            // Обновляем состояние команд
            LoadDataCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();

            // Создаём новый токен отмены
            _cts = new CancellationTokenSource();

            // Имитация долгой загрузки (например, запрос к API)
            await Task.Delay(3000, _cts.Token); // 3 секунды

            // Если отмены не было, обновляем данные
            if (!_cts.Token.IsCancellationRequested)
            {
                LoadedData = $"Данные загружены: {DateTime.Now:T}";
            }
        }
        catch (OperationCanceledException)
        {
            LoadedData = "Загрузка отменена";
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

    private void CancelLoad()
    {
        _cts?.Cancel();
    }
}