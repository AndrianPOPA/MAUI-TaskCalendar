using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Schdeuler.ViewModel
{
    public class TodoService
    {
        private readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "todos.json");

        public void SaveTodoItems(ObservableCollection<TodoItem> todos)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(todos, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving todos: {ex.Message}");
            }
        }

        public ObservableCollection<TodoItem> LoadTodoItems()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json);
                    return new ObservableCollection<TodoItem>(todoItems ?? new List<TodoItem>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading todos: {ex.Message}");
            }

            return new ObservableCollection<TodoItem>();
        }

        public void UpdateTodoItemCompletionStatus(string appointmentId, bool isCompleted)
        {
            try
            {
                var todos = LoadTodoItems();
                var todoItem = todos.FirstOrDefault(t => t.AppointmentId == appointmentId);

                if (todoItem != null)
                {
                    todoItem.IsCompleted = isCompleted;
                    SaveTodoItems(todos);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating todo status: {ex.Message}");
            }
        }
    }
}