using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Start;

/// <summary>
/// Interaction logic for HostResponse.xaml
/// </summary>
public partial class HostingSection : UserControl
{
    private readonly Action onClick;

    public HostingSection(Action onClick)
    {
        InitializeComponent();
        this.onClick = onClick;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        onClick.Invoke();
    }
}

