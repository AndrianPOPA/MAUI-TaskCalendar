// <summary>
// Provides data management services for scheduler events including persistence, 
// CRUD operations, and property management for both events and tasks.
// </summary>

using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Schdeuler.ViewModel
{
    /// <summary>
    /// Service responsible for managing scheduler events including data persistence,
    /// CRUD operations, and property management for both events and tasks.
    /// </summary>
    class SchedulerEventService
    {
        /// <summary>
        /// File path for storing event data in JSON format.
        /// Uses the application's data directory for cross-platform compatibility.
        /// </summary>
        private readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "unified_events.json");

        /// <summary>
        /// Collection of all scheduler appointments.
        /// </summary>
        private ObservableCollection<SchedulerAppointment> _appointments;

        /// <summary>
        /// Dictionary storing additional properties for each appointment (completion status, date presence, event type).
        /// Uses appointment ID as key and a tuple of properties as value.
        /// </summary>
        private Dictionary<string, (bool IsCompleted, bool HasDate, bool IsEvent)> _eventProperties = new Dictionary<string, (bool, bool, bool)>();

        /// <summary>
        /// Saves all appointments to a JSON file for persistence.
        /// Converts SchedulerAppointment objects to serializable format before saving.
        /// </summary>
        private void SaveEventsToFile()
        {
            // Convert appointments to serializable format, preserving additional properties
            var serializableEvents = _appointments.Select(a => {
                bool isCompleted = false;
                bool hasDate = true;
                bool isEvent = true;

                // Retrieve additional properties if they exist for this appointment
                if (a.Id != null && _eventProperties.ContainsKey(a.Id.ToString()))
                {
                    var props = _eventProperties[a.Id.ToString()];
                    isCompleted = props.IsCompleted;
                    hasDate = props.HasDate;
                    isEvent = props.IsEvent;
                }

                return new SerializableSchedulerAppointment(a, isCompleted, hasDate, isEvent);
            }).ToList();

            // Serialize and save to file
            var json = JsonSerializer.Serialize(serializableEvents);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads appointments from a JSON file if it exists.
        /// Initializes empty collections if no data file is found.
        /// </summary>
        private void LoadEventsFromFile()
        {
            // Check if data file exists before attempting to load
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var serializableEvents = JsonSerializer.Deserialize<List<SerializableSchedulerAppointment>>(json) ?? new List<SerializableSchedulerAppointment>();
                _appointments = new ObservableCollection<SchedulerAppointment>();
                _eventProperties = new Dictionary<string, (bool, bool, bool)>();

                // Convert serialized events back to SchedulerAppointment objects
                foreach (var serEvent in serializableEvents)
                {
                    var appointment = serEvent.ToSchedulerAppointment();
                    _appointments.Add(appointment);

                    // Restore additional properties if they exist
                    if (appointment.Id != null)
                    {
                        _eventProperties[appointment.Id.ToString()] = (serEvent.IsCompleted, serEvent.HasDate, serEvent.IsEvent);
                    }
                }
            }
            else
            {
                // Initialize empty collections if no data file exists
                _appointments = new ObservableCollection<SchedulerAppointment>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerEventService"/> class.
        /// Automatically loads existing events from persistent storage.
        /// </summary>
        public SchedulerEventService()
        {
            LoadEventsFromFile();
        }

        /// <summary>
        /// Gets all scheduler events.
        /// </summary>
        /// <returns>Observable collection of all scheduler appointments.</returns>
        public ObservableCollection<SchedulerAppointment> GetAllEvents()
        {
            return _appointments;
        }

        /// <summary>
        /// Gets the completion status of all events.
        /// </summary>
        /// <returns>Dictionary mapping appointment IDs to completion status.</returns>
        public Dictionary<string, bool> GetCompletionStatus()
        {
            return _eventProperties.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.IsCompleted
            );
        }

        /// <summary>
        /// Adds a new event to the scheduler.
        /// </summary>
        /// <param name="startTime">The start time of the event.</param>
        /// <param name="endTime">The end time of the event.</param>
        /// <param name="subject">The subject/title of the event.</param>
        /// <param name="background">The background color for the event.</param>
        /// <param name="isCompleted">Whether the event is marked as completed (default: false).</param>
        /// <param name="isEvent">Whether the item is an event (true) or task (false) (default: true).</param>
        public void AddEvent(DateTime startTime, DateTime endTime, string subject, Color background, bool isCompleted = false, bool isEvent = true)
        {
            var appointment = new SchedulerAppointment
            {
                StartTime = startTime,
                EndTime = endTime,
                Subject = subject,
                Background = background
            };

            _appointments.Add(appointment);

            // Store additional properties if the appointment has an ID
            if (appointment.Id != null)
            {
                _eventProperties[appointment.Id.ToString()] = (isCompleted, true, isEvent);
            }

            SaveEventsToFile();
        }

        /// <summary>
        /// Removes an event from the scheduler.
        /// </summary>
        /// <param name="appointment">The appointment to remove.</param>
        public void RemoveEvent(SchedulerAppointment appointment)
        {
            if (_appointments.Remove(appointment))
            {
                // Also remove associated properties to prevent memory leaks
                if (appointment.Id != null && _eventProperties.ContainsKey(appointment.Id.ToString()))
                {
                    _eventProperties.Remove(appointment.Id.ToString());
                }

                SaveEventsToFile();
            }
        }

        /// <summary>
        /// Updates an existing event with new information.
        /// </summary>
        /// <param name="appointment">The appointment to update.</param>
        /// <param name="startTime">The new start time.</param>
        /// <param name="endTime">The new end time.</param>
        /// <param name="subject">The new subject.</param>
        /// <param name="background">The new background color.</param>
        public void UpdateEvent(SchedulerAppointment appointment, DateTime startTime, DateTime endTime, string subject, Color background)
        {
            if (_appointments.Contains(appointment))
            {
                appointment.StartTime = startTime;
                appointment.EndTime = endTime;
                appointment.Subject = subject;
                appointment.Background = background;
                SaveEventsToFile();
            }
        }

        /// <summary>
        /// Updates the completion status of an appointment.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to update.</param>
        /// <param name="isCompleted">The new completion status.</param>
        public void UpdateCompletionStatus(string appointmentId, bool isCompleted)
        {
            if (_eventProperties.ContainsKey(appointmentId))
            {
                var current = _eventProperties[appointmentId];
                _eventProperties[appointmentId] = (isCompleted, current.HasDate, current.IsEvent);
            }
            else
            {
                // Add properties if they don't exist (fallback for data consistency)
                _eventProperties.Add(appointmentId, (isCompleted, true, true)); 
            }

            SaveEventsToFile();
        }

        /// <summary>
        /// Updates all properties of an appointment.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to update.</param>
        /// <param name="isCompleted">The new completion status.</param>
        /// <param name="hasDate">Whether the appointment has a date.</param>
        /// <param name="isEvent">Whether the appointment is an event (true) or task (false).</param>
        public void UpdateEventProperties(string appointmentId, bool isCompleted, bool hasDate, bool isEvent = false)
        {
            if (_eventProperties.ContainsKey(appointmentId))
            {
                var current = _eventProperties[appointmentId];
                _eventProperties[appointmentId] = (isCompleted, hasDate, isEvent);
            }
            else
            {
                // Add properties if they don't exist (fallback for data consistency)
                _eventProperties.Add(appointmentId, (isCompleted, hasDate, isEvent));
            }

            SaveEventsToFile();
        }

        /// <summary>
        /// Gets all events that occur on a specific date.
        /// </summary>
        /// <param name="date">The date to filter events by.</param>
        /// <returns>List of events occurring on the specified date.</returns>
        public List<SchedulerAppointment> GetEventsByDate(DateTime date)
        {
            return _appointments
                .Where(a => a.StartTime.Date <= date.Date && a.EndTime.Date >= date.Date)
                .ToList();
        }

        /// <summary>
        /// Saves all changes to persistent storage.
        /// </summary>
        public void SaveChanges()
        {
            SaveEventsToFile();
        }

        /// <summary>
        /// Adds a task without an associated date.
        /// </summary>
        /// <param name="subject">The subject of the task.</param>
        public void AddTaskWithoutDate(string subject)
        {
            var now = DateTime.Now;
            var appointment = new SchedulerAppointment
            {
                StartTime = now,
                EndTime = now.AddHours(1),
                Subject = subject,
                Background = new SolidColorBrush(Colors.Orange)
            };

            _appointments.Add(appointment);

            // Store properties indicating this is a dateless task
            if (appointment.Id != null)
            {
                _eventProperties[appointment.Id.ToString()] = (false, false, false); 
            }

            SaveEventsToFile();
        }

        /// <summary>
        /// Gets the properties of a specific appointment.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment.</param>
        /// <returns>Tuple containing completion status, date presence, and event type.</returns>
        public (bool IsCompleted, bool HasDate, bool IsEvent) GetEventProperties(string appointmentId)
        {
            if (_eventProperties.ContainsKey(appointmentId))
            {
                return _eventProperties[appointmentId];
            }
            // Return default values if properties don't exist
            return (false, true, true); 
        }

        /// <summary>
        /// Gets all todo items (non-event appointments) from the collection.
        /// </summary>
        /// <returns>Observable collection of todo items.</returns>
        public ObservableCollection<TodoItem> GetAllTodoItems()
        {
            var todoItems = new ObservableCollection<TodoItem>();

            // Filter appointments to include only those marked as tasks (not events)
            foreach (var appointment in _appointments)
            {
                bool isCompleted = false;
                bool hasDate = true;
                bool isEvent = true; 

                // Retrieve properties if they exist
                if (appointment.Id != null && _eventProperties.ContainsKey(appointment.Id.ToString()))
                {
                    var props = _eventProperties[appointment.Id.ToString()];
                    isCompleted = props.IsCompleted;
                    hasDate = props.HasDate;
                    isEvent = props.IsEvent;
                }

                // Include only non-events (tasks) in the todo list
                if (!isEvent)
                {
                    var todoItem = new TodoItem(appointment)
                    {
                        IsCompleted = isCompleted,
                        HasDate = hasDate,
                        IsEvent = isEvent
                    };
                    todoItems.Add(todoItem);
                }
            }

            return todoItems;
        }
    }
}