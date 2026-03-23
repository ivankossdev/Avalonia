using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MyFirstAvaloniaApp.Models;
using MyFirstAvaloniaApp.Services;
using MyFirstAvaloniaApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


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

    public IAsyncRelayCommand ExportNotesCommand { get; } = null!;
    public IAsyncRelayCommand ImportNotesCommand { get; } = null!;

    public MainViewModel(IDialogService dialogService, INoteService noteService, MainWindow mainWindow)
    {
        _dialogService = dialogService;
        _noteService = noteService;
        _mainWindow = mainWindow;

        LoadNotesCommand = new AsyncRelayCommand(LoadNotesAsync);
        AddNoteCommand = new AsyncRelayCommand(AddNoteAsync);
        EditNoteCommand = new AsyncRelayCommand<Note>(EditNoteAsync);
        DeleteNoteCommand = new AsyncRelayCommand<Note>(DeleteNoteAsync);

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
    public async Task ExportNotesAsync()
    {
        var topLevel = TopLevel.GetTopLevel(_mainWindow);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Экспорт заметок",
            SuggestedFileName = "notes.xml",
            DefaultExtension = "xml",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("XML файлы")
                {
                    Patterns = new[] { "*.xml" }
                }
            }
        });

        if (file == null) return;

        try
        {
            await _noteService.ExportToXmlAsync(file.Path.AbsolutePath);
            await _dialogService.ShowInfoAsync("Экспорт", "Заметки успешно экспортированы.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Ошибка экспорта", ex.Message);
        }
    }

    public async Task ImportNotesAsync()
    {
        var topLevel = TopLevel.GetTopLevel(_mainWindow);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Импорт заметок",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("XML файлы")
                {
                    Patterns = new[] { "*.xml" }
                }
            }
        });

        if (files == null || files.Count == 0) return;

        var file = files[0];

        try
        {
            await _noteService.ImportFromXmlAsync(file.Path.AbsolutePath);
            await LoadNotesAsync(); // обновляем список
            await _dialogService.ShowInfoAsync("Импорт", "Заметки успешно импортированы.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Ошибка импорта", ex.Message);
        }
    }
}