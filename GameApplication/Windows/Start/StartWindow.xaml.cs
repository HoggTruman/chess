﻿using System.Windows;


namespace GameApplication.Windows.Start;


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