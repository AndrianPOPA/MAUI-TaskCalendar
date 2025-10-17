// <summary>
// Main page of the application containing the UI logic for calendar and todo functionality.
// Handles user interactions, data binding, and coordination between UI elements and view models.
// </summary>

using Microsoft.Maui.Controls;
using Schdeuler.ViewModel;
using Syncfusion.Maui.Core.Carousel;
using Syncfusion.Maui.Core.Internals;
using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Schdeuler
{
    /// <summary>
    /// Main page of the application that implements both calendar and todo functionality.
    /// Manages UI state, user interactions, and data binding between views and view models.
    /// </summary>
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        /// <summary>
        /// The currently selected appointment in the scheduler.
        /// </summary>
        private SchedulerAppointment selectedAppointment;

        /// <summary>
        /// Flag indicating whether the calendar view is currently visible.
        /// </summary>
        private bool _isCalendarVisible = true;

        /// <summary>
        /// Gets or sets whether the calendar view is visible.
        /// Notifies listeners when the value changes.
        /// </summary>
        public bool IsCalendarVisible
        {
            get => _isCalendarVisible;
            set
            {
                if (_isCalendarVisible != value)
                {
                    _isCalendarVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag indicating whether the todo view is currently visible.
        /// </summary>
        private bool _isTodoVisible = false;

        /// <summary>
        /// Gets or sets whether the todo view is visible.
        /// Notifies listeners when the value changes.
        /// </summary>
        public bool IsTodoVisible
        {
            get => _isTodoVisible;
            set
            {
                if (_isTodoVisible != value)
                {
                    _isTodoVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag indicating whether the current input mode is for events (true) or tasks (false).
        /// </summary>
        private bool _isEventMode = true; 

        /// <summary>
        /// Handles the event toggle button click, setting the input mode to event creation.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnEventToggleClicked(object sender, EventArgs e)
        {
            _isEventMode = true;
            UpdateToggleUI();
        }

        /// <summary>
        /// Handles the task toggle button click, setting the input mode to task creation.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnTaskToggleClicked(object sender, EventArgs e)
        {
            _isEventMode = false;
            UpdateToggleUI();
        }

        /// <summary>
        /// Handles changes to the "has date" checkbox for tasks.
        /// Shows or hides the date/time selection controls based on the checkbox state.
        /// </summary>
        /// <param name="sender">The checkbox that changed.</param>
        /// <param name="e">Event arguments containing the new checked state.</param>
        private void OnHasDateCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            taskDateTimeSection.IsVisible = e.Value;
        }

        /// <summary>
        /// Updates the UI elements of the event/task toggle buttons to reflect the current mode.
        /// Changes button colors and visibility of date/time controls.
        /// </summary>
        private void UpdateToggleUI()
        {
            if (_isEventMode)
            {
                // Set UI for event mode
                eventToggleButton.BackgroundColor = Colors.Orange;
                eventToggleButton.TextColor = Colors.White;
                taskToggleButton.BackgroundColor = Colors.LightGray;
                taskToggleButton.TextColor = Colors.Black;

                dateTimeSection.IsVisible = true;

                hasDateCheckbox.IsVisible = false;
                hasDateLabel.IsVisible = false;
                taskDateTimeSection.IsVisible = false;
            }
            else
            {
                // Set UI for task mode
                eventToggleButton.BackgroundColor = Colors.LightGray;
                eventToggleButton.TextColor = Colors.Black;
                taskToggleButton.BackgroundColor = Colors.Orange;
                taskToggleButton.TextColor = Colors.White;

                dateTimeSection.IsVisible = false;

                hasDateCheckbox.IsVisible = true;
                hasDateLabel.IsVisible = true;

                taskDateTimeSection.IsVisible = hasDateCheckbox.IsChecked;
            }
        }

        /// <summary>
        /// Background color for the calendar tab button.
        /// </summary>
        private Color _calendarTabColor = Colors.DarkOrange;

        /// <summary>
        /// Gets or sets the background color for the calendar tab button.
        /// Notifies listeners when the value changes.
        /// </summary>
        public Color CalendarTabColor
        {
            get => _calendarTabColor;
            set
            {
                if (_calendarTabColor != value)
                {
                    _calendarTabColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Background color for the todo tab button.
        /// </summary>
        private Color _todoTabColor = Colors.Orange;

        /// <summary>
        /// Gets or sets the background color for the todo tab button.
        /// Notifies listeners when the value changes.
        /// </summary>
        public Color TodoTabColor
        {
            get => _todoTabColor;
            set
            {
                if (_todoTabColor != value)
                {
                    _todoTabColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Collection of all todo events.
        /// </summary>
        private ObservableCollection<TodoItem> _todoEvents = new ObservableCollection<TodoItem>();

        /// <summary>
        /// Gets or sets the collection of all todo events.
        /// Notifies listeners when the value changes.
        /// </summary>
        public ObservableCollection<TodoItem> TodoEvents
        {
            get => _todoEvents;
            set
            {
                _todoEvents = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of active (incomplete) todo events.
        /// </summary>
        private ObservableCollection<TodoItem> _activeTodoEvents = new ObservableCollection<TodoItem>();

        /// <summary>
        /// Gets or sets the collection of active todo events.
        /// Notifies listeners when the value changes.
        /// </summary>
        public ObservableCollection<TodoItem> ActiveTodoEvents
        {
            get => _activeTodoEvents;
            set
            {
                _activeTodoEvents = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of completed todo events.
        /// </summary>
        private ObservableCollection<TodoItem> _completedTodoEvents = new ObservableCollection<TodoItem>();

        /// <summary>
        /// Gets or sets the collection of completed todo events.
        /// Notifies listeners when the value changes.
        /// </summary>
        public ObservableCollection<TodoItem> CompletedTodoEvents
        {
            get => _completedTodoEvents;
            set
            {
                _completedTodoEvents = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Service for managing todo item persistence.
        /// </summary>
        private TodoService _todoService;

        /// <summary>
        /// Saves the current input as a new event.
        /// Validates date/time constraints before saving.
        /// </summary>
        private void SaveAsEvent()
        {
            var startDateTime = startDatePicker.Date + startTimePicker.Time;
            var endDateTime = endDatePicker.Date + endTimePicker.Time;

            // Validate that end time is after start time
            if (endDateTime <= startDateTime)
            {
                DisplayAlert("Eroare", "Data de sfârșit trebuie să fie după data de început", "OK");
                return;
            }

            viewModel.AddNewEvent(
                startDateTime,
                endDateTime,
                subjectEntry.Text,
                GetRandomColor(), 
                false,  // isCompleted
                true,   // hasDate
                true    // isEvent
            );

            subjectEntry.Text = string.Empty;
        }

        /// <summary>
        /// Saves the current input as a new task.
        /// Handles both dated and dateless tasks based on user selection.
        /// </summary>
        private void SaveAsTask()
        {
            if (hasDateCheckbox.IsChecked)
            {
                // Handle dated task
                var startDateTime = taskStartDatePicker.Date + taskStartTimePicker.Time;
                var endDateTime = taskEndDatePicker.Date + taskEndTimePicker.Time;

                // Validate that end time is after start time
                if (endDateTime <= startDateTime)
                {
                    DisplayAlert("Eroare", "Data de sfârșit trebuie să fie după data de început", "OK");
                    return;
                }

                viewModel.AddNewEvent(
                    startDateTime,
                    endDateTime,
                    subjectEntry.Text,
                    GetRandomColor(),
                    false, // isCompleted
                    true,  // hasDate
                    false  // isEvent
                );
            }
            else
            {
                // Handle dateless task
                viewModel.AddTaskWithoutDate(subjectEntry.Text);
            }

            subjectEntry.Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// Sets up the UI, data binding, and initial state.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            AppShell.SetNavBarIsVisible(this, false);
            this.BindingContext = this;

            _todoService = new TodoService();
            var now = DateTime.Now;
            startDatePicker.Date = now.Date;
            startTimePicker.Time = new TimeSpan(now.Hour, now.Minute, 0);

            endDatePicker.Date = now.Date;
            endTimePicker.Time = new TimeSpan(now.Hour + 1, now.Minute, 0);

            // Load todo events when scheduler is ready
            scheduler.Loaded += (s, e) => LoadTodoEvents();

            scheduler.AllowAppointmentDrag = true;
        }

        /// <summary>
        /// Loads todo events from the view model and organizes them into active and completed collections.
        /// </summary>
        private void LoadTodoEvents()
        {
            TodoEvents.Clear();
            ActiveTodoEvents.Clear();
            CompletedTodoEvents.Clear();

            var allTodos = viewModel.GetAllTodoItems();

            foreach (var todoItem in allTodos)
            {
                TodoEvents.Add(todoItem);

                if (todoItem.IsCompleted)
                {
                    CompletedTodoEvents.Add(todoItem);
                }
                else
                {
                    ActiveTodoEvents.Add(todoItem);
                }
            }
        }

        /// <summary>
        /// Handles the calendar tab button click, switching to calendar view.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCalendarTabClicked(object sender, EventArgs e)
        {
            IsCalendarVisible = true;
            IsTodoVisible = false;
            CalendarTabColor = Colors.DarkOrange;
            TodoTabColor = Colors.Orange;
        }

        /// <summary>
        /// Handles the todo tab button click, switching to todo view.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnTodoTabClicked(object sender, EventArgs e)
        {
            IsCalendarVisible = false;
            IsTodoVisible = true;
            CalendarTabColor = Colors.Orange;
            TodoTabColor = Colors.DarkOrange;
            LoadTodoEvents(); 
        }

        /// <summary>
        /// Handles the add event/task button click, showing the input popup.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAddEventClicked(object sender, EventArgs e)
        {
            _isEventMode = true;

            subjectEntry.Text = string.Empty;
            hasDateCheckbox.IsChecked = false;

            UpdateToggleUI();

            var now = DateTime.Now;
            startDatePicker.Date = now.Date;
            startTimePicker.Time = new TimeSpan(now.Hour, now.Minute, 0);
            endDatePicker.Date = now.Date;
            endTimePicker.Time = new TimeSpan(now.Hour + 1, now.Minute, 0);

            taskStartDatePicker.Date = now.Date;
            taskStartTimePicker.Time = new TimeSpan(now.Hour, now.Minute, 0);
            taskEndDatePicker.Date = now.Date;
            taskEndTimePicker.Time = new TimeSpan(now.Hour + 1, now.Minute, 0);

            eventPopup.IsVisible = true;
        }

        /// <summary>
        /// Handles the cancel button click in the add event popup, hiding the popup.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCancelClicked(object sender, EventArgs e)
        {
            eventPopup.IsVisible = false;
        }

        /// <summary>
        /// Handles the save button click in the add event popup, validating input and saving the event/task.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSaveEventClicked(object sender, EventArgs e)
        {
            // Validate that subject is not empty
            if (string.IsNullOrWhiteSpace(subjectEntry.Text))
            {
                DisplayAlert("Eroare", "Introduceți un subiect", "OK");
                return;
            }

            // Save as event or task based on current mode
            if (_isEventMode)
            {
                SaveAsEvent();
            }
            else
            {
                SaveAsTask();
            }

            eventPopup.IsVisible = false;
        }

        /// <summary>
        /// Handles taps on scheduler appointments, showing the edit popup for the selected appointment.
        /// </summary>
        /// <param name="sender">The scheduler control.</param>
        /// <param name="e">Event arguments containing information about the tap.</param>
        private void OnSchedulerTapped(object sender, Syncfusion.Maui.Scheduler.SchedulerTappedEventArgs e)
        {
            // Check if an appointment was tapped
            if (e.Appointments != null && e.Appointments.Count > 0)
            {
                selectedAppointment = e.Appointments[0] as SchedulerAppointment;

                if (selectedAppointment != null)
                {
                    // Populate edit popup with appointment data
                    editSubjectEntry.Text = selectedAppointment.Subject;

                    editStartDatePicker.Date = selectedAppointment.StartTime.Date;
                    editStartTimePicker.Time = selectedAppointment.StartTime.TimeOfDay;

                    editEndDatePicker.Date = selectedAppointment.EndTime.Date;
                    editEndTimePicker.Time = selectedAppointment.EndTime.TimeOfDay;

                    editEventPopup.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handles the cancel button click in the edit event popup, hiding the popup.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCancelEditClicked(object sender, EventArgs e)
        {
            editEventPopup.IsVisible = false;
        }

        /// <summary>
        /// Handles the update button click in the edit event popup, validating input and updating the appointment.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUpdateEventClicked(object sender, EventArgs e)
        {
            // Validate that subject is not empty
            if (string.IsNullOrWhiteSpace(editSubjectEntry.Text))
            {
                DisplayAlert("Eroare", "Introduceți un subiect pentru eveniment", "OK");
                return;
            }

            var startDateTime = editStartDatePicker.Date + editStartTimePicker.Time;
            var endDateTime = editEndDatePicker.Date + editEndTimePicker.Time;

            // Validate that end time is after start time
            if (endDateTime <= startDateTime)
            {
                DisplayAlert("Eroare", "Data de sfârșit trebuie să fie după data de început", "OK");
                return;
            }

            // Update the appointment if one is selected
            if (selectedAppointment != null)
            {
                if (selectedAppointment.Background is SolidColorBrush solidColorBrush)
                {
                    var oldSubject = selectedAppointment.Subject;
                    selectedAppointment.StartTime = startDateTime;
                    selectedAppointment.EndTime = endDateTime;
                    selectedAppointment.Subject = editSubjectEntry.Text;

                    viewModel.SaveChanges();

                    UpdateTodoItem(selectedAppointment, oldSubject);
                }
                else
                {
                    viewModel.UpdateEvent(
                        selectedAppointment,
                        startDateTime,
                        endDateTime,
                        editSubjectEntry.Text,
                        Colors.Blue
                    );

                    UpdateTodoItem(selectedAppointment, "");
                }
            }

            editEventPopup.IsVisible = false;
        }

        /// <summary>
        /// Handles the delete button click in the edit event popup, confirming and deleting the appointment.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnDeleteEventClicked(object sender, EventArgs e)
        {
            // Confirm deletion with user
            bool answer = await DisplayAlert("Confirmare", "Sunteți sigur că doriți să ștergeți acest eveniment?", "Da", "Nu");

            if (answer && selectedAppointment != null)
            {
                RemoveTodoItem(selectedAppointment);

                viewModel.RemoveEvent(selectedAppointment);

                editEventPopup.IsVisible = false;
            }
        }

        /// <summary>
        /// Updates a todo item to match the properties of a scheduler appointment.
        /// </summary>
        /// <param name="appointment">The appointment to update from.</param>
        /// <param name="oldSubject">The previous subject of the appointment.</param>
        private void UpdateTodoItem(SchedulerAppointment appointment, string oldSubject)
        {
            foreach (var todoItem in TodoEvents)
            {
                // Find the matching todo item
                if ((string.IsNullOrEmpty(oldSubject) && todoItem.AppointmentId == appointment.Id?.ToString()) ||
                    (!string.IsNullOrEmpty(oldSubject) && todoItem.Subject == oldSubject))
                {
                    bool wasCompleted = todoItem.IsCompleted;
                    todoItem.UpdateFromAppointment(appointment);

                    // Move item between active and completed collections if completion status changed
                    if (wasCompleted && !CompletedTodoEvents.Contains(todoItem))
                    {
                        ActiveTodoEvents.Remove(todoItem);
                        CompletedTodoEvents.Add(todoItem);
                    }
                    else if (!wasCompleted && !ActiveTodoEvents.Contains(todoItem))
                    {
                        CompletedTodoEvents.Remove(todoItem);
                        ActiveTodoEvents.Add(todoItem);
                    }

                    break;
                }
            }

            _todoService.SaveTodoItems(TodoEvents);
        }

        /// <summary>
        /// Removes a todo item that corresponds to a deleted scheduler appointment.
        /// </summary>
        /// <param name="appointment">The appointment that was deleted.</param>
        private void RemoveTodoItem(SchedulerAppointment appointment)
        {
            string appointmentId = appointment.Id?.ToString();
            TodoItem itemToRemove = null;

            // Find the todo item to remove
            foreach (var todoItem in TodoEvents)
            {
                if (todoItem.AppointmentId == appointmentId)
                {
                    itemToRemove = todoItem;
                    break;
                }
            }

            // Remove the item from all collections
            if (itemToRemove != null)
            {
                TodoEvents.Remove(itemToRemove);

                if (ActiveTodoEvents.Contains(itemToRemove))
                {
                    ActiveTodoEvents.Remove(itemToRemove);
                }

                if (CompletedTodoEvents.Contains(itemToRemove))
                {
                    CompletedTodoEvents.Remove(itemToRemove);
                }

                _todoService.SaveTodoItems(TodoEvents);
            }
        }

        /// <summary>
        /// Handles changes to todo item completion checkboxes, updating the item's status and moving it between collections.
        /// </summary>
        /// <param name="sender">The checkbox that changed.</param>
        /// <param name="e">Event arguments containing the new checked state.</param>
        private void OnTodoItemCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem todoItem)
            {
                todoItem.IsCompleted = e.Value;

                viewModel.UpdateEventProperties(todoItem.AppointmentId, e.Value, todoItem.HasDate, todoItem.IsEvent);

                // Move item between active and completed collections based on new status
                if (e.Value)
                {
                    if (ActiveTodoEvents.Contains(todoItem))
                    {
                        ActiveTodoEvents.Remove(todoItem);
                        CompletedTodoEvents.Add(todoItem);
                    }
                }
                else 
                {
                    if (CompletedTodoEvents.Contains(todoItem))
                    {
                        CompletedTodoEvents.Remove(todoItem);
                        ActiveTodoEvents.Add(todoItem);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a random color from a predefined set of colors.
        /// </summary>
        /// <returns>A randomly selected color.</returns>
        private Color GetRandomColor()
        {
            var random = new Random();
            var colors = new List<Color>
            {
                Colors.Blue,
                Colors.Green,
                Colors.Orange,
                Colors.Purple,
                Colors.Pink,
                Colors.Teal
            };

            return colors[random.Next(colors.Count)];
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Represents a todo item that can be displayed in the todo list.
    /// Implements INotifyPropertyChanged to support data binding.
    /// </summary>
    public class TodoItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the ID of the associated appointment.
        /// </summary>
        public string AppointmentId { get; set; }

        /// <summary>
        /// Gets or sets the subject/title of the todo item.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the start time of the todo item.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the todo item.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets whether the todo item has an associated date.
        /// Default is true to maintain backward compatibility.
        /// </summary>
        public bool HasDate { get; set; } = true; 

        /// <summary>
        /// Gets or sets whether the todo item is an event (true) or task (false).
        /// Default is false since this is specifically a todo item.
        /// </summary>
        public bool IsEvent { get; set; } = false;

        /// <summary>
        /// Flag indicating whether the todo item is completed.
        /// </summary>
        private bool _isCompleted;

        /// <summary>
        /// Gets or sets whether the todo item is completed.
        /// Notifies listeners when the value changes.
        /// </summary>
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a formatted string representing the date and time information.
        /// Shows "Fără dată" for dateless items or formatted date/time for dated items.
        /// </summary>
        public string DateTimeInfo
        {
            get
            {
                if (!HasDate)
                    return "Fără dată";
                return $"{StartTime.ToString("dd/MM/yyyy HH:mm")} - {EndTime.ToString("HH:mm")}";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoItem"/> class.
        /// </summary>
        public TodoItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoItem"/> class from a scheduler appointment.
        /// </summary>
        /// <param name="appointment">The appointment to create the todo item from.</param>
        /// <param name="isEvent">Whether the item is an event (true) or task (false).</param>
        public TodoItem(SchedulerAppointment appointment, bool isEvent = true)
        {
            AppointmentId = appointment.Id?.ToString();
            Subject = appointment.Subject;
            StartTime = appointment.StartTime;
            EndTime = appointment.EndTime;
            IsCompleted = false;
            HasDate = true;
            IsEvent = isEvent; 
        }

        /// <summary>
        /// Updates this todo item with properties from a scheduler appointment.
        /// </summary>
        /// <param name="appointment">The appointment to update from.</param>
        public void UpdateFromAppointment(SchedulerAppointment appointment)
        {
            Subject = appointment.Subject;
            StartTime = appointment.StartTime;
            EndTime = appointment.EndTime;
            HasDate = true;
            OnPropertyChanged(nameof(Subject));
            OnPropertyChanged(nameof(StartTime));
            OnPropertyChanged(nameof(EndTime));
            OnPropertyChanged(nameof(DateTimeInfo));
            OnPropertyChanged(nameof(HasDate));
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates this todo item with properties from a task appointment.
        /// </summary>
        /// <param name="appointment">The appointment to update from.</param>
        public void UpdateFromTask(SchedulerAppointment appointment)
        {
            Subject = appointment.Subject;
            StartTime = appointment.StartTime;
            EndTime = appointment.EndTime;
            OnPropertyChanged(nameof(Subject));
            OnPropertyChanged(nameof(DateTimeInfo));
        }
    }
}