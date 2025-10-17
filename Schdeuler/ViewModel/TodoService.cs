// <summary>
// Provides data persistence services for todo items using JSON serialization.
// Handles saving, loading, and updating todo item completion status.
// </summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Schdeuler.ViewModel
{
    /// <summary>
    /// Service responsible for managing todo item data persistence.
    /// Handles saving, loading, and updating todo items in JSON format.
    /// </summary>
    public class TodoService
    {
        /// <summary>
        /// File path for storing todo item data in JSON format.
        /// Uses the application's data directory for cross-platform compatibility.
        /// </summary>
        private readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "todos.json");

        /// <summary>
        /// Saves a collection of todo items to a JSON file.
        /// </summary>
        /// <param name="todos">The collection of todo items to save.</param>
        public void SaveTodoItems(ObservableCollection<TodoItem> todos)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true // Format JSON for readability
                };
                var json = JsonSerializer.Serialize(todos, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                Console.WriteLine($"Error saving todos: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads todo items from a JSON file.
        /// </summary>
        /// <returns>A collection of todo items, or an empty collection if loading fails or file doesn't exist.</returns>
        public ObservableCollection<TodoItem> LoadTodoItems()
        {
            try
            {
                // Check if data file exists before attempting to load
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json);
                    return new ObservableCollection<TodoItem>(todoItems ?? new List<TodoItem>());
                }
            }
            catch (Exception ex)
            {
                // Log error but return empty collection as fallback
                Console.WriteLine($"Error loading todos: {ex.Message}");
            }

            // Return empty collection if file doesn't exist or loading fails
            return new ObservableCollection<TodoItem>();
        }

        /// <summary>
        /// Updates the completion status of a specific todo item.
        /// </summary>
        /// <param name="appointmentId">The ID of the todo item to update.</param>
        /// <param name="isCompleted">The new completion status.</param>
        public void UpdateTodoItemCompletionStatus(string appointmentId, bool isCompleted)
        {
            try
            {
                var todos = LoadTodoItems();
                var todoItem = todos.FirstOrDefault(t => t.AppointmentId == appointmentId);

                // Update the item if found
                if (todoItem != null)
                {
                    todoItem.IsCompleted = isCompleted;
                    SaveTodoItems(todos);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                Console.WriteLine($"Error updating todo status: {ex.Message}");
            }
        }
    }
}