using System.Windows;
using CsharpWpfStarter.App.ViewModels;

namespace CsharpWpfStarter.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
