// <summary>
// Provides the main view model for the application, managing calendar events and todo items.
// Acts as a bridge between the UI and the data services.
// </summary>

using Schdeuler.ViewModel;
using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.ObjectModel;

namespace Schdeuler
{
    /// <summary>
    /// Main view model that coordinates between the UI and the event service.
    /// Manages both calendar events and todo items in a unified interface.
    /// </summary>
    public class ControlViewModel
    {
        /// <summary>
        /// Service responsible for managing event data persistence and business logic.
        /// </summary>
        private SchedulerEventService _eventService;

        /// <summary>
        /// Collection of all scheduler appointments (events and tasks).
        /// </summary>
        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel"/> class.
        /// Sets up the event service and loads all existing events.
        /// </summary>
        public ControlViewModel()
        {
            _eventService = new SchedulerEventService();
            SchedulerEvents = _eventService.GetAllEvents();
        }

        /// <summary>
        /// Collection of scheduler events that have associated dates (filter out dateless tasks).
        /// Lazy-loaded to improve performance on initial load.
        /// </summary>
        private ObservableCollection<SchedulerAppointment> _schedulerEventsWithDates;

        /// <summary>
        /// Gets the collection of scheduler events that have associated dates.
        /// This property ensures the collection is initialized and filtered on first access.
        /// </summary>
        public ObservableCollection<SchedulerAppointment> SchedulerEventsWithDates
        {
            get
            {
                // Initialize the filtered collection if it hasn't been created yet
                if (_schedulerEventsWithDates == null)
                {
                    _schedulerEventsWithDates = new ObservableCollection<SchedulerAppointment>();
                    UpdateFilteredEvents();
                }
                return _schedulerEventsWithDates;
            }
        }

        /// <summary>
        /// Updates the filtered collection to include only events that have dates.
        /// This method clears the existing collection and repopulates it based on current events.
        /// </summary>
        private void UpdateFilteredEvents()
        {
            _schedulerEventsWithDates.Clear();

            // Iterate through all events and add only those with dates to the filtered collection
            foreach (var appointment in SchedulerEvents)
            {
                string id = appointment.Id?.ToString();
                if (id != null && _eventService.GetEventProperties(id).HasDate)
                {
                    _schedulerEventsWithDates.Add(appointment);
                }
            }
        }

        /// <summary>
        /// Adds a new event or task to the scheduler.
        /// </summary>
        /// <param name="startTime">The start time of the event/task.</param>
        /// <param name="endTime">The end time of the event/task.</param>
        /// <param name="subject">The subject/title of the event/task.</param>
        /// <param name="background">The background color for the event/task.</param>
        /// <param name="isCompleted">Whether the item is marked as completed (default: false).</param>
        /// <param name="hasDate">Whether the item has an associated date (default: true).</param>
        /// <param name="isEvent">Whether the item is an event (true) or a task (false) (default: true).</param>
        public void AddNewEvent(DateTime startTime, DateTime endTime, string subject, Color background, bool isCompleted = false, bool hasDate = true, bool isEvent = true)
        {
            _eventService.AddEvent(startTime, endTime, subject, background, isCompleted, isEvent);

            // If the item doesn't have a date, update its properties after creation
            if (!hasDate && SchedulerEvents.Count > 0)
            {
                var lastEvent = SchedulerEvents[SchedulerEvents.Count - 1];
                if (lastEvent.Id != null)
                {
                    _eventService.UpdateEventProperties(lastEvent.Id.ToString(), isCompleted, hasDate, isEvent);
                }
            }
            UpdateFilteredEvents();
        }

        /// <summary>
        /// Removes an appointment from the scheduler.
        /// </summary>
        /// <param name="appointment">The appointment to remove.</param>
        public void RemoveEvent(SchedulerAppointment appointment)
        {
            _eventService.RemoveEvent(appointment);
        }

        /// <summary>
        /// Updates an existing appointment with new information.
        /// </summary>
        /// <param name="appointment">The appointment to update.</param>
        /// <param name="startTime">The new start time.</param>
        /// <param name="endTime">The new end time.</param>
        /// <param name="subject">The new subject.</param>
        /// <param name="background">The new background color.</param>
        public void UpdateEvent(SchedulerAppointment appointment, DateTime startTime, DateTime endTime, string subject, Color background)
        {
            _eventService.UpdateEvent(appointment, startTime, endTime, subject, background);
        }

        /// <summary>
        /// Updates the completion status of an appointment.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to update.</param>
        /// <param name="isCompleted">The new completion status.</param>
        public void UpdateCompletionStatus(string appointmentId, bool isCompleted)
        {
            _eventService.UpdateCompletionStatus(appointmentId, isCompleted);
        }

        /// <summary>
        /// Updates the properties of an appointment.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to update.</param>
        /// <param name="isCompleted">The new completion status.</param>
        /// <param name="hasDate">Whether the appointment has a date.</param>
        /// <param name="isEvent">Whether the appointment is an event (true) or task (false).</param>
        public void UpdateEventProperties(string appointmentId, bool isCompleted, bool hasDate, bool isEvent)
        {
            _eventService.UpdateEventProperties(appointmentId, isCompleted, hasDate, isEvent);
        }

        /// <summary>
        /// Gets all todo items from the event service.
        /// </summary>
        /// <returns>A collection of todo items.</returns>
        public ObservableCollection<TodoItem> GetAllTodoItems()
        {
            return _eventService.GetAllTodoItems();
        }

        /// <summary>
        /// Gets the completion status of all events.
        /// </summary>
        /// <returns>A dictionary mapping appointment IDs to their completion status.</returns>
        public Dictionary<string, bool> GetCompletionStatus()
        {
            return _eventService.GetCompletionStatus();
        }

        /// <summary>
        /// Adds a task without an associated date.
        /// </summary>
        /// <param name="subject">The subject of the task.</param>
        public void AddTaskWithoutDate(string subject)
        {
            _eventService.AddTaskWithoutDate(subject);
        }

        /// <summary>
        /// Saves all changes to persistent storage.
        /// </summary>
        public void SaveChanges()
        {
            _eventService.SaveChanges();
        }
    }
}