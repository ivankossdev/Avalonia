using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace MyApp;

public partial class MainWindow : Window
{   private int clickCount = 0;
    public MainWindow()
    {
        InitializeComponent();
        
        Button? dynamicButton = this.FindControl<Button>("DynamicButton");
        if (dynamicButton is not null)
        {
            dynamicButton.Click += Button_Click;
        }
    }

        // Обработчик события Click
        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                clickCount++;
                button.Content = $"Кликнуто: {clickCount} раз";
                
                // Меняем цвет при каждом клике
                button.Background = clickCount % 2 == 0 
                    ? Brushes.LightBlue 
                    : Brushes.LightGreen;
                System.Console.WriteLine("Click");
            }
        }
}