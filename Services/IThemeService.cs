using System;
namespace MyFirstAvaloniaApp.Services;

public interface IThemeService
{
    bool IsDarkTheme { get; }
    void ToggleTheme();
    event EventHandler<bool> ThemeChanged;
}