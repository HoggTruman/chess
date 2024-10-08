﻿using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Start;

/// <summary>
/// Interaction logic for HostButton.xaml
/// </summary>
public partial class HostButton : UserControl
{
    private readonly Action onClick;

    public HostButton(Action onClick)
    {
        InitializeComponent();
        this.onClick = onClick;
    }

    private void HostGame_Click(object sender, RoutedEventArgs e)
    {
        onClick.Invoke();
    }
}

