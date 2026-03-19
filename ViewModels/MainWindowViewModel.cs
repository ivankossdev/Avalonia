using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyFirstAvaloniaApp.Models;
using MyFirstAvaloniaApp.Services;
using MyFirstAvaloniaApp.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyFirstAvaloniaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly INoteService _noteService;
    private readonly MainWindow _mainWindow; // ссылка на главное окно для открытия диалогов

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

    public MainViewModel(IDialogService dialogService, INoteService noteService, MainWindow mainWindow)
    {
        _dialogService = dialogService;
        _noteService = noteService;
        _mainWindow = mainWindow;

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
        var newNote = new Note { Title = "Новая заметка", Content = "", CreatedAt = System.DateTime.Now };
        var id = await _noteService.SaveNoteAsync(newNote);
        newNote.Id = id;
        Notes.Insert(0, newNote);
        SelectedNote = newNote;
    }

    private async Task EditNoteAsync(Note? note)
    {
        if (note == null) return;

        // Создаём копию заметки для редактирования
        var editCopy = new Note
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt
        };

        var editVM = new EditNoteViewModel(editCopy);
        var editWindow = new EditNoteWindow
        {
            DataContext = editVM
        };

        editVM.CloseAction = (result) =>
        {
            if (result)
            {
                // Сохраняем изменения в оригинальную заметку
                note.Title = editCopy.Title;
                note.Content = editCopy.Content;

                // Обновляем заметку в БД
                Task.Run(async () => await _noteService.SaveNoteAsync(note));

                // Обновляем элемент в коллекции (чтобы UI обновился)
                var index = Notes.IndexOf(note);
                if (index >= 0)
                {
                    Notes[index] = note; // перезапись элемента вызывает обновление списка
                }
            }
            editWindow.Close();
        };

        await editWindow.ShowDialog(_mainWindow);
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