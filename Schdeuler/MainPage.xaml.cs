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
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private SchedulerAppointment selectedAppointment;

        private bool _isCalendarVisible = true;
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

        private bool _isTodoVisible = false;
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

        private bool _isEventMode = true; 

        private void OnEventToggleClicked(object sender, EventArgs e)
        {
            _isEventMode = true;
            UpdateToggleUI();
        }

        private void OnTaskToggleClicked(object sender, EventArgs e)
        {
            _isEventMode = false;
            UpdateToggleUI();
        }

       

        private void OnHasDateCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            taskDateTimeSection.IsVisible = e.Value;
        }

        private void UpdateToggleUI()
        {
            if (_isEventMode)
            {
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
                // Mod Task
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


        private Color _calendarTabColor = Colors.DarkOrange;
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

        private Color _todoTabColor = Colors.Orange;
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

        private ObservableCollection<TodoItem> _todoEvents = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> TodoEvents
        {
            get => _todoEvents;
            set
            {
                _todoEvents = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TodoItem> _activeTodoEvents = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> ActiveTodoEvents
        {
            get => _activeTodoEvents;
            set
            {
                _activeTodoEvents = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TodoItem> _completedTodoEvents = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> CompletedTodoEvents
        {
            get => _completedTodoEvents;
            set
            {
                _completedTodoEvents = value;
                OnPropertyChanged();
            }
        }

        private TodoService _todoService;

        private void SaveAsEvent()
        {
            var startDateTime = startDatePicker.Date + startTimePicker.Time;
            var endDateTime = endDatePicker.Date + endTimePicker.Time;

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
                false,
                true,  
                true   
            );

            subjectEntry.Text = string.Empty;
        }

        private void SaveAsTask()
        {
            if (hasDateCheckbox.IsChecked)
            {
                var startDateTime = taskStartDatePicker.Date + taskStartTimePicker.Time;
                var endDateTime = taskEndDatePicker.Date + taskEndTimePicker.Time;

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
                    false, 
                    true, 
                    false 
                );
            }
            else
            {
                viewModel.AddTaskWithoutDate(subjectEntry.Text);
            }

            subjectEntry.Text = string.Empty;
        }

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

            scheduler.Loaded += (s, e) => LoadTodoEvents();

            scheduler.AllowAppointmentDrag = true;
        }

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

        private void OnCalendarTabClicked(object sender, EventArgs e)
        {
            IsCalendarVisible = true;
            IsTodoVisible = false;
            CalendarTabColor = Colors.DarkOrange;
            TodoTabColor = Colors.Orange;
        }

        private void OnTodoTabClicked(object sender, EventArgs e)
        {
            IsCalendarVisible = false;
            IsTodoVisible = true;
            CalendarTabColor = Colors.Orange;
            TodoTabColor = Colors.DarkOrange;
            LoadTodoEvents(); 
        }

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

        private void OnCancelClicked(object sender, EventArgs e)
        {
            eventPopup.IsVisible = false;
        }

        private void OnSaveEventClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(subjectEntry.Text))
            {
                DisplayAlert("Eroare", "Introduceți un subiect", "OK");
                return;
            }

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

        private void OnSchedulerTapped(object sender, Syncfusion.Maui.Scheduler.SchedulerTappedEventArgs e)
        {
            if (e.Appointments != null && e.Appointments.Count > 0)
            {
                selectedAppointment = e.Appointments[0] as SchedulerAppointment;

                if (selectedAppointment != null)
                {
                    editSubjectEntry.Text = selectedAppointment.Subject;

                    editStartDatePicker.Date = selectedAppointment.StartTime.Date;
                    editStartTimePicker.Time = selectedAppointment.StartTime.TimeOfDay;

                    editEndDatePicker.Date = selectedAppointment.EndTime.Date;
                    editEndTimePicker.Time = selectedAppointment.EndTime.TimeOfDay;

                    editEventPopup.IsVisible = true;
                }
            }
        }

        private void OnCancelEditClicked(object sender, EventArgs e)
        {
            editEventPopup.IsVisible = false;
        }

        private void OnUpdateEventClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(editSubjectEntry.Text))
            {
                DisplayAlert("Eroare", "Introduceți un subiect pentru eveniment", "OK");
                return;
            }

            var startDateTime = editStartDatePicker.Date + editStartTimePicker.Time;
            var endDateTime = editEndDatePicker.Date + editEndTimePicker.Time;

            if (endDateTime <= startDateTime)
            {
                DisplayAlert("Eroare", "Data de sfârșit trebuie să fie după data de început", "OK");
                return;
            }

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

        private async void OnDeleteEventClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmare", "Sunteți sigur că doriți să ștergeți acest eveniment?", "Da", "Nu");

            if (answer && selectedAppointment != null)
            {
                RemoveTodoItem(selectedAppointment);

                viewModel.RemoveEvent(selectedAppointment);

                editEventPopup.IsVisible = false;
            }
        }

        private void UpdateTodoItem(SchedulerAppointment appointment, string oldSubject)
        {
            foreach (var todoItem in TodoEvents)
            {
                if ((string.IsNullOrEmpty(oldSubject) && todoItem.AppointmentId == appointment.Id?.ToString()) ||
                    (!string.IsNullOrEmpty(oldSubject) && todoItem.Subject == oldSubject))
                {
                    bool wasCompleted = todoItem.IsCompleted;
                    todoItem.UpdateFromAppointment(appointment);

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
        private void RemoveTodoItem(SchedulerAppointment appointment)
        {
            string appointmentId = appointment.Id?.ToString();
            TodoItem itemToRemove = null;

            foreach (var todoItem in TodoEvents)
            {
                if (todoItem.AppointmentId == appointmentId)
                {
                    itemToRemove = todoItem;
                    break;
                }
            }

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

        private void OnTodoItemCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem todoItem)
            {
                todoItem.IsCompleted = e.Value;

                viewModel.UpdateEventProperties(todoItem.AppointmentId, e.Value, todoItem.HasDate, todoItem.IsEvent);

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TodoItem : INotifyPropertyChanged
    {
        public string AppointmentId { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool HasDate { get; set; } = true; 
        public bool IsEvent { get; set; } = false;

        private bool _isCompleted;
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

        public string DateTimeInfo
        {
            get
            {
                if (!HasDate)
                    return "Fără dată";
                return $"{StartTime.ToString("dd/MM/yyyy HH:mm")} - {EndTime.ToString("HH:mm")}";
            }
        }

        public TodoItem()
        {
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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