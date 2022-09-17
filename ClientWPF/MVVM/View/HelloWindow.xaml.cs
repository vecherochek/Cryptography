using System.Windows;
using System.Windows.Input;

namespace ClientWPF.MVVM.View;

public partial class HelloWindow : Window
{
    public HelloWindow()
    {
        InitializeComponent();
    }
    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void Minimaze_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow != null)
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }

    private void Maximaze_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow != null)
            Application.Current.MainWindow.WindowState =
                Application.Current.MainWindow.WindowState != WindowState.Maximized
                    ? WindowState.Maximized
                    : WindowState.Normal;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}