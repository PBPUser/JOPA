﻿using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.IO;

namespace JVOS.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        App.MainWindowInstance = this;
        if(File.Exists("fullscreen"))
                WindowState = WindowState.FullScreen;
    }
}
