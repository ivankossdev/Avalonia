using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyFirstAvaloniaApp.Models;
using MyFirstAvaloniaApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyFirstAvaloniaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly INoteService _noteService;

    [ObservableProperty]
    private ObservableCollection<Note> _notes = new();

    [ObservableProperty]
    private Note? _selectedNote;

    [ObservableProperty]
    private bool _isLoading;

    public IAsyncRelayCommand LoadNotesCommand { get; }
    public IAsyncRelayCommand AddNoteCommand { get; }
    public IAsyncRelayCommand<Note> EditNoteCommand { get; }
    public IAsyncRelayCommand<Note> DeleteNoteCommand { get; }

    public MainViewModel(IDialogService dialogService, INoteService noteService)
    {
        _dialogService = dialogService;
        _noteService = noteService;

        LoadNotesCommand = new AsyncRelayCommand(LoadNotesAsync);
        AddNoteCommand = new AsyncRelayCommand(AddNoteAsync);
        EditNoteCommand = new AsyncRelayCommand<Note>(EditNoteAsync);
        DeleteNoteCommand = new AsyncRelayCommand<Note>(DeleteNoteAsync);

        // Загружаем заметки при старте
        LoadNotesCommand.Execute(null);
    }

    private async Task LoadNotesAsync()
    {
        IsLoading = true;
        var notes = await _noteService.GetNotesAsync();
        Notes.Clear();
        foreach (var note in notes)
            Notes.Add(note);
        IsLoading = false;
    }

    private async Task AddNoteAsync()
    {
        // Создаём новую заметку
        var newNote = new Note { Title = "Новая заметка", Content = "", CreatedAt = System.DateTime.Now };
        var id = await _noteService.SaveNoteAsync(newNote);
        newNote.Id = id;
        Notes.Insert(0, newNote); // Добавляем в начало списка
        SelectedNote = newNote;
        // Можно сразу открыть редактирование (опционально)
    }

    private async Task EditNoteAsync(Note? note)
    {
        if (note == null) return;

        // Здесь можно открыть диалог редактирования
        // Пока просто покажем информационное сообщение
        await _dialogService.ShowInfoAsync("Редактирование", $"Редактирование заметки: {note.Title}");
    }

    private async Task DeleteNoteAsync(Note? note)
    {
        if (note == null) return;

        var confirmed = await _dialogService.ShowConfirmationAsync("Подтверждение", 
            $"Удалить заметку \"{note.Title}\"?");
        if (confirmed)
        {
            await _noteService.DeleteNoteAsync(note.Id);
            Notes.Remove(note);
        }
    }
}