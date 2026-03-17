using Avalonia.Controls;
using Avalonia.Styling;
using MyFirstAvaloniaApp.ViewModels;

namespace MyFirstAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Classes.Add("DarkTheme");
    }


}
