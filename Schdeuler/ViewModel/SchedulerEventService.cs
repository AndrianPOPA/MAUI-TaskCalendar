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
    class SchedulerEventService
    {
        private readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "unified_events.json");
        private ObservableCollection<SchedulerAppointment> _appointments;
        private Dictionary<string, (bool IsCompleted, bool HasDate, bool IsEvent)> _eventProperties = new Dictionary<string, (bool, bool, bool)>();

        private void SaveEventsToFile()
        {
            var serializableEvents = _appointments.Select(a => {
                bool isCompleted = false;
                bool hasDate = true;
                bool isEvent = true;

                if (a.Id != null && _eventProperties.ContainsKey(a.Id.ToString()))
                {
                    var props = _eventProperties[a.Id.ToString()];
                    isCompleted = props.IsCompleted;
                    hasDate = props.HasDate;
                    isEvent = props.IsEvent;
                }

                return new SerializableSchedulerAppointment(a, isCompleted, hasDate, isEvent);
            }).ToList();

            var json = JsonSerializer.Serialize(serializableEvents);
            File.WriteAllText(filePath, json);
        }

        private void LoadEventsFromFile()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var serializableEvents = JsonSerializer.Deserialize<List<SerializableSchedulerAppointment>>(json) ?? new List<SerializableSchedulerAppointment>();
                _appointments = new ObservableCollection<SchedulerAppointment>();
                _eventProperties = new Dictionary<string, (bool, bool, bool)>();

                foreach (var serEvent in serializableEvents)
                {
                    var appointment = serEvent.ToSchedulerAppointment();
                    _appointments.Add(appointment);

                    if (appointment.Id != null)
                    {
                        _eventProperties[appointment.Id.ToString()] = (serEvent.IsCompleted, serEvent.HasDate, serEvent.IsEvent);
                    }
                }
            }
            else
            {
                _appointments = new ObservableCollection<SchedulerAppointment>();
            }
        }


        public SchedulerEventService()
        {
            LoadEventsFromFile();
        }

        public ObservableCollection<SchedulerAppointment> GetAllEvents()
        {
            return _appointments;
        }

        public Dictionary<string, bool> GetCompletionStatus()
        {
            return _eventProperties.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.IsCompleted
            );
        }

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

            if (appointment.Id != null)
            {
                _eventProperties[appointment.Id.ToString()] = (isCompleted, true, isEvent);
            }

            SaveEventsToFile();
        }

        public void RemoveEvent(SchedulerAppointment appointment)
        {
            if (_appointments.Remove(appointment))
            {
                // Eliminăm și starea de completare
                if (appointment.Id != null && _eventProperties.ContainsKey(appointment.Id.ToString()))
                {
                    _eventProperties.Remove(appointment.Id.ToString());
                }

                SaveEventsToFile();
            }
        }

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

        public void UpdateCompletionStatus(string appointmentId, bool isCompleted)
        {
            if (_eventProperties.ContainsKey(appointmentId))
            {
                var current = _eventProperties[appointmentId];
                _eventProperties[appointmentId] = (isCompleted, current.HasDate, current.IsEvent);
            }
            else
            {
                _eventProperties.Add(appointmentId, (isCompleted, true, true)); 
            }

            SaveEventsToFile();
        }

        public void UpdateEventProperties(string appointmentId, bool isCompleted, bool hasDate, bool isEvent = false)
        {
            if (_eventProperties.ContainsKey(appointmentId))
            {
                var current = _eventProperties[appointmentId];
                _eventProperties[appointmentId] = (isCompleted, hasDate, isEvent);
            }
            else
            {
                _eventProperties.Add(appointmentId, (isCompleted, hasDate, isEvent));
            }

            SaveEventsToFile();
        }


        public List<SchedulerAppointment> GetEventsByDate(DateTime date)
        {
            return _appointments
                .Where(a => a.StartTime.Date <= date.Date && a.EndTime.Date >= date.Date)
                .ToList();
        }

        public void SaveChanges()
        {
            SaveEventsToFile();
        }

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

            if (appointment.Id != null)
            {
                _eventProperties[appointment.Id.ToString()] = (false, false, false); 
            }

            SaveEventsToFile();
        }

        public (bool IsCompleted, bool HasDate, bool IsEvent) GetEventProperties(string appointmentId)
        {
            if (_eventProperties.ContainsKey(appointmentId))
            {
                return _eventProperties[appointmentId];
            }
            return (false, true, true); 
        }

        public ObservableCollection<TodoItem> GetAllTodoItems()
        {
            var todoItems = new ObservableCollection<TodoItem>();

            foreach (var appointment in _appointments)
            {
                bool isCompleted = false;
                bool hasDate = true;
                bool isEvent = true; 

                if (appointment.Id != null && _eventProperties.ContainsKey(appointment.Id.ToString()))
                {
                    var props = _eventProperties[appointment.Id.ToString()];
                    isCompleted = props.IsCompleted;
                    hasDate = props.HasDate;
                    isEvent = props.IsEvent;
                }

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