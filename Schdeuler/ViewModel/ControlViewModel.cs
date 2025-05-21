using Schdeuler.ViewModel;
using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.ObjectModel;

namespace Schdeuler
{
    public class ControlViewModel
    {
        private SchedulerEventService _eventService;
        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; }


        public ControlViewModel()
        {
            _eventService = new SchedulerEventService();
            SchedulerEvents = _eventService.GetAllEvents();
        }

        private ObservableCollection<SchedulerAppointment> _schedulerEventsWithDates;
        public ObservableCollection<SchedulerAppointment> SchedulerEventsWithDates
        {
            get
            {
                if (_schedulerEventsWithDates == null)
                {
                    _schedulerEventsWithDates = new ObservableCollection<SchedulerAppointment>();
                    UpdateFilteredEvents();
                }
                return _schedulerEventsWithDates;
            }
        }

        private void UpdateFilteredEvents()
        {
            _schedulerEventsWithDates.Clear();

            foreach (var appointment in SchedulerEvents)
            {
                string id = appointment.Id?.ToString();
                if (id != null && _eventService.GetEventProperties(id).HasDate)
                {
                    _schedulerEventsWithDates.Add(appointment);
                }
            }
        }

        public void AddNewEvent(DateTime startTime, DateTime endTime, string subject, Color background, bool isCompleted = false, bool hasDate = true, bool isEvent = true)
        {
            _eventService.AddEvent(startTime, endTime, subject, background, isCompleted, isEvent);

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

        public void RemoveEvent(SchedulerAppointment appointment)
        {
            _eventService.RemoveEvent(appointment);
        }

        public void UpdateEvent(SchedulerAppointment appointment, DateTime startTime, DateTime endTime, string subject, Color background)
        {
            _eventService.UpdateEvent(appointment, startTime, endTime, subject, background);
        }

        public void UpdateCompletionStatus(string appointmentId, bool isCompleted)
        {
            _eventService.UpdateCompletionStatus(appointmentId, isCompleted);
        }

        public void UpdateEventProperties(string appointmentId, bool isCompleted, bool hasDate, bool isEvent)
        {
            _eventService.UpdateEventProperties(appointmentId, isCompleted, hasDate, isEvent);
        }

        public ObservableCollection<TodoItem> GetAllTodoItems()
        {
            return _eventService.GetAllTodoItems();
        }

        public Dictionary<string, bool> GetCompletionStatus()
        {
            return _eventService.GetCompletionStatus();
        }

        public void AddTaskWithoutDate(string subject)
        {
            _eventService.AddTaskWithoutDate(subject);
        }

        public void SaveChanges()
        {
            _eventService.SaveChanges();
        }
    }
}