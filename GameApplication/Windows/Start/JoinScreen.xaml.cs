using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Start;

/// <summary>
/// Interaction logic for JoinScreen.xaml
/// </summary>
public partial class JoinScreen : UserControl
{
    #region Fields

    private readonly Window window;

    #endregion



    #region Constructors

    public JoinScreen(Window window)
    {
        InitializeComponent();
        this.window = window;
    }

    #endregion



    #region Event Handlers

    private void JoinGame_Click(object sender, RoutedEventArgs e)
    {
        var code = CodeTextBox.Text;
        if (code.Length != 20)
        {
            // make separate method for validation
            JoinStatusTextBox.Text = "Invalid Code Provided";
            return;
        }
        else
        {
            JoinStatusTextBox.Text = "joining... / no room exists with code provided";
            // try to access room on server
            // if bad tell user no room available with code xyz
        }
        
    }


    private void Back_Click(object sender, RoutedEventArgs e)
    {
        StartScreen startScreen = new(window);
        window.Content = startScreen;
    }

    #endregion
}

