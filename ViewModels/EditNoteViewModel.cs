using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyFirstAvaloniaApp.Models;
using System;

namespace MyFirstAvaloniaApp.ViewModels;

public partial class EditNoteViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _content;

    public Note OriginalNote { get; }

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public Action<bool>? CloseAction { get; set; }

    public EditNoteViewModel(Note note)
    {
        OriginalNote = note;
        _title = note.Title;
        _content = note.Content;

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Save()
    {
        OriginalNote.Title = Title;
        OriginalNote.Content = Content;
        CloseAction?.Invoke(true);
    }

    private void Cancel()
    {
        CloseAction?.Invoke(false);
    }
}