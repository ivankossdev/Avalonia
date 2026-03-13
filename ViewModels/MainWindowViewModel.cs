using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyFirstAvaloniaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _logText = "История нажатий:\n";

    [RelayCommand]
    private void Button1()
    {
        LogText += "Нажата кнопка 1\n";
    }

    [RelayCommand]
    private void Button2()
    {
        LogText += "Нажата кнопка 2\n";
    }

    [RelayCommand]
    private void Clear()
    {
        LogText = "История нажатий:\n";
    }
}