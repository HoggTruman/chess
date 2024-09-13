using GameLogic.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client;

/// <summary>
/// Interaction logic for HostScreen.xaml
/// </summary>
public partial class HostScreen : UserControl
{
    private readonly Window window;

    private PieceColor hostColor = PieceColor.White;

    private bool waitingForOpponent = false;

    private readonly SolidColorBrush highlightBrush = new(Colors.LightGreen);

    public HostScreen(Window window)
    {
        InitializeComponent();
        this.window = window;
        HostStatusContainer.Content = new HostButton(HandleCancelClicked);
    }


    private void HandleHostClicked()
    {
        HostStatusContainer.Content = new HostingSection(HandleCancelClicked);
        waitingForOpponent = true;
    }

    private void HandleCancelClicked()
    {
        HostStatusContainer.Content = new HostButton(HandleHostClicked);
        waitingForOpponent = false;
    }


    private void WhiteOption_Click(object sender, RoutedEventArgs e)
    {
        if (waitingForOpponent == false)
        {
            hostColor = PieceColor.White;
            WhiteOptionHighlight.Background = highlightBrush;
            BlackOptionHighlight.Background = null;
        }

    }

    private void BlackOption_Click(object sender, RoutedEventArgs e)
    {
        if (waitingForOpponent == false)
        {
            hostColor = PieceColor.Black;
            BlackOptionHighlight.Background = highlightBrush;
            WhiteOptionHighlight.Background = null;
        }
    }

    private void HostGame_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        StartScreen startScreen = new(window);
        window.Content = startScreen;
    }
}

