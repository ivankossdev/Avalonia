using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MyFirstAvaloniaApp.Services;

public class ThemeService : IThemeService
{
    private bool _isDarkTheme;
    private readonly Application? _app;

    public ThemeService()
    {
        _app = Application.Current;
        _isDarkTheme = false; // светлая тема по умолчанию
        ApplyTheme();
    }

    public bool IsDarkTheme => _isDarkTheme;

    public void ToggleTheme()
    {
        _isDarkTheme = !_isDarkTheme;
        ApplyTheme();
        ThemeChanged?.Invoke(this, _isDarkTheme);
    }

    public event EventHandler<bool>? ThemeChanged;

    private void ApplyTheme()
    {
        if (_app == null) return;

        // Загружаем словарь ресурсов из файла темы
        var uri = new Uri($"avares://MyFirstAvaloniaApp/Themes/{(IsDarkTheme ? "DarkTheme.axaml" : "LightTheme.axaml")}");
        var themeDict = AvaloniaXamlLoader.Load(uri) as ResourceDictionary;
        if (themeDict == null) return;

        // Очищаем текущие объединённые словари и добавляем новый
        _app.Resources.MergedDictionaries.Clear();
        _app.Resources.MergedDictionaries.Add(themeDict);
    }
}