using System.Collections.Generic;
using System.Threading.Tasks;
using MyFirstAvaloniaApp.Models;

namespace MyFirstAvaloniaApp.Services;

public interface INoteService
{
    Task InitializeDatabaseAsync();
    Task<List<Note>> GetNotesAsync();
    Task<Note?> GetNoteAsync(int id);
    Task<int> SaveNoteAsync(Note note);
    Task<int> DeleteNoteAsync(int id);
}