using System;
using System.Text.Json.Serialization;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls; 
using Syncfusion.Maui.Scheduler;

namespace Schdeuler.ViewModel
{
    public class SerializableSchedulerAppointment
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public string BackgroundHex { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasDate { get; set; } = true; 
        public bool IsEvent { get; set; } = true;

        [JsonConstructor]
        public SerializableSchedulerAppointment() { }

        public SerializableSchedulerAppointment(SchedulerAppointment appointment, bool isCompleted = false, bool hasDate = true, bool isEvent = true)
        {
            StartTime = appointment.StartTime;
            EndTime = appointment.EndTime;
            Subject = appointment.Subject;
            BackgroundHex = GetColorHex(appointment.Background);
            IsCompleted = isCompleted;
            HasDate = hasDate;
            IsEvent = isEvent;
        }

        public SerializableSchedulerAppointment(TodoItem todoItem)
        {
            StartTime = todoItem.StartTime;
            EndTime = todoItem.EndTime;
            Subject = todoItem.Subject;
            BackgroundHex = "#FFA500"; 
            IsCompleted = todoItem.IsCompleted;
            HasDate = todoItem.HasDate;
        }

        public SchedulerAppointment ToSchedulerAppointment()
        {
            return new SchedulerAppointment
            {
                StartTime = StartTime,
                EndTime = EndTime,
                Subject = Subject,
                Background = GetBrushFromHex(BackgroundHex)
            };
        }

        public TodoItem ToTodoItem()
        {
            return new TodoItem
            {
                AppointmentId = Guid.NewGuid().ToString(),
                StartTime = StartTime,
                EndTime = EndTime,
                Subject = Subject,
                IsCompleted = IsCompleted,
                HasDate = HasDate
            };
        }

        private string GetColorHex(Brush brush)
        {
            if (brush is SolidColorBrush solidColorBrush)
            {
                var color = solidColorBrush.Color;
                return $"#{(int)(color.Alpha * 255):X2}{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
            }
            return "#FFFFA500"; 
        }

        private Brush GetBrushFromHex(string hex)
        {
            return new SolidColorBrush(Color.FromArgb(hex));
        }
    }
}