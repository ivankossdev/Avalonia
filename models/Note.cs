using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MyFirstAvaloniaApp.Models;

public partial class Note : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _content = string.Empty;

    [ObservableProperty]
    private DateTime _createdAt;
}