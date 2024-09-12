using GameLogic;
using GameLogic.Enums;
using System.Windows;


namespace Client;


/// <summary>
/// Interaction logic for StartWindow.xaml
/// </summary>
public partial class StartWindow : Window
{
    public StartWindow()
    {
        InitializeComponent();
        //Visibility = Visibility.Hidden;
        LoadStartScreen();
    }


    private void LoadStartScreen()
    {
        StartScreen startScreen = new(this);
        Content = startScreen;
    }
}