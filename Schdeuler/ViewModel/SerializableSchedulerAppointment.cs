// <summary>
// Provides serialization capabilities for SchedulerAppointment objects,
// allowing them to be saved to and loaded from JSON files.
// </summary>

using System;
using System.Text.Json.Serialization;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls; 
using Syncfusion.Maui.Scheduler;

namespace Schdeuler.ViewModel
{
    /// <summary>
    /// Serializable representation of a SchedulerAppointment with additional properties.
    /// Used for persisting appointment data to JSON files.
    /// </summary>
    public class SerializableSchedulerAppointment
    {
        /// <summary>
        /// Gets or sets the start time of the appointment.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the appointment.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the subject/title of the appointment.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the background color of the appointment as a hex string.
        /// </summary>
        public string BackgroundHex { get; set; }

        /// <summary>
        /// Gets or sets whether the appointment is marked as completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets whether the appointment has an associated date.
        /// Default is true to maintain backward compatibility.
        /// </summary>
        public bool HasDate { get; set; } = true; 

        /// <summary>
        /// Gets or sets whether the appointment is an event (true) or task (false).
        /// Default is true to maintain backward compatibility.
        /// </summary>
        public bool IsEvent { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableSchedulerAppointment"/> class.
        /// Required for JSON deserialization.
        /// </summary>
        [JsonConstructor]
        public SerializableSchedulerAppointment() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableSchedulerAppointment"/> class
        /// from an existing SchedulerAppointment.
        /// </summary>
        /// <param name="appointment">The SchedulerAppointment to serialize.</param>
        /// <param name="isCompleted">Whether the appointment is completed.</param>
        /// <param name="hasDate">Whether the appointment has a date.</param>
        /// <param name="isEvent">Whether the appointment is an event or task.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableSchedulerAppointment"/> class
        /// from an existing TodoItem.
        /// </summary>
        /// <param name="todoItem">The TodoItem to serialize.</param>
        public SerializableSchedulerAppointment(TodoItem todoItem)
        {
            StartTime = todoItem.StartTime;
            EndTime = todoItem.EndTime;
            Subject = todoItem.Subject;
            BackgroundHex = "#FFA500"; // Orange color for tasks
            IsCompleted = todoItem.IsCompleted;
            HasDate = todoItem.HasDate;
        }

        /// <summary>
        /// Converts this serializable appointment back to a SchedulerAppointment.
        /// </summary>
        /// <returns>A new SchedulerAppointment with the same properties.</returns>
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

        /// <summary>
        /// Converts this serializable appointment to a TodoItem.
        /// </summary>
        /// <returns>A new TodoItem with the same properties.</returns>
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

        /// <summary>
        /// Converts a Brush to its hex color representation.
        /// </summary>
        /// <param name="brush">The brush to convert.</param>
        /// <returns>Hex string representation of the color.</returns>
        private string GetColorHex(Brush brush)
        {
            if (brush is SolidColorBrush solidColorBrush)
            {
                var color = solidColorBrush.Color;
                return $"#{(int)(color.Alpha * 255):X2}{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
            }
            return "#FFFFA500"; // Default to orange if conversion fails
        }

        /// <summary>
        /// Converts a hex color string to a Brush.
        /// </summary>
        /// <param name="hex">The hex color string.</param>
        /// <returns>A SolidColorBrush with the specified color.</returns>
        private Brush GetBrushFromHex(string hex)
        {
            return new SolidColorBrush(Color.FromArgb(hex));
        }
    }
}