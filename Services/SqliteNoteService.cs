using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MyFirstAvaloniaApp.Models;

namespace MyFirstAvaloniaApp.Services;

public class SqliteNoteService : INoteService
{
    private readonly string _connectionString;

    public SqliteNoteService(string databasePath = "notes.db")
    {
        _connectionString = $"Data Source={databasePath}";
    }

    public async Task InitializeDatabaseAsync()
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            )";
        await createTableCmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Note>> GetNotesAsync()
    {
        var notes = new List<Note>();

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT Id, Title, Content, CreatedAt FROM Notes ORDER BY CreatedAt DESC";

        await using var reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            notes.Add(new Note
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Content = reader.GetString(2),
                CreatedAt = DateTime.Parse(reader.GetString(3))
            });
        }

        return notes;
    }

    public async Task<Note?> GetNoteAsync(int id)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT Id, Title, Content, CreatedAt FROM Notes WHERE Id = @id";
        selectCmd.Parameters.AddWithValue("@id", id);

        await using var reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Note
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Content = reader.GetString(2),
                CreatedAt = DateTime.Parse(reader.GetString(3))
            };
        }

        return null;
    }

    public async Task<int> SaveNoteAsync(Note note)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        if (note.Id == 0)
        {
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO Notes (Title, Content, CreatedAt)
                VALUES (@title, @content, @createdAt);
                SELECT last_insert_rowid();";
            insertCmd.Parameters.AddWithValue("@title", note.Title);
            insertCmd.Parameters.AddWithValue("@content", note.Content);
            insertCmd.Parameters.AddWithValue("@createdAt", note.CreatedAt.ToString("o"));

            var newId = await insertCmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }
        else
        {
            var updateCmd = connection.CreateCommand();
            updateCmd.CommandText = @"
                UPDATE Notes
                SET Title = @title, Content = @content, CreatedAt = @createdAt
                WHERE Id = @id";
            updateCmd.Parameters.AddWithValue("@title", note.Title);
            updateCmd.Parameters.AddWithValue("@content", note.Content);
            updateCmd.Parameters.AddWithValue("@createdAt", note.CreatedAt.ToString("o"));
            updateCmd.Parameters.AddWithValue("@id", note.Id);

            return await updateCmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> DeleteNoteAsync(int id)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Notes WHERE Id = @id";
        deleteCmd.Parameters.AddWithValue("@id", id);

        return await deleteCmd.ExecuteNonQueryAsync();
    }
}