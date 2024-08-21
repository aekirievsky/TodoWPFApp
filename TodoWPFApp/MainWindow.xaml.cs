using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoWPFApp.Models;
using TodoWPFApp.DTOs;


namespace TodoWPFApp
{
    public partial class MainWindow : Window
    {
        public List<int> Years { get; set; }
        public int SelectedYear { get; set; }

        private readonly HttpClient _client;

        public ObservableCollection<TodoModel> NotesForSelectedDate { get; set; }
        public ObservableCollection<TodoModel> AllNotes { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _client = new HttpClient() { BaseAddress = new Uri("http://localhost:5221") };

            NotesForSelectedDate = new ObservableCollection<TodoModel>();
            AllNotes = new ObservableCollection<TodoModel>();

            Years = new List<int>();

            for (int year = 2020; year <= 2027; year++)
            {
                Years.Add(year);
            }

            SelectedYear = DateTime.Now.Year;
            DateTime startDate = new DateTime(SelectedYear, 1, 1);
            calendar.DisplayDate = startDate;
            calendar.SelectedDate = startDate;

            DownloadNotesFromDataBase();
            UpdateCalendar();
            UpdateNotesForSelectedDate(startDate);
            DataContext = this;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

        }

        private void lblNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtNote.Focus();
        }

        private void txtNote_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNote.Text) && txtNote.Text.Length > 0)
            {
                lblNote.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblNote.Visibility = Visibility.Visible;
            }
        }

        private void lblTime_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtTime.Focus();
        }

        private void txtTime_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTime.Text) && txtTime.Text.Length > 0)
            {
                lblTime.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblTime.Visibility = Visibility.Visible;
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedYear > Years.Min())
            {
                SelectedYear--;
                UpdateCalendar();
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedYear < Years.Max())
            {
                SelectedYear++;
                UpdateCalendar();
            }
        }

        private void YearButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBtn = (Button)sender;
            SelectedYear = (int)clickedBtn.Content;
            UpdateCalendar();
        }

        private void UpdateCalendar()
        {
            int monthIndex = calendar.DisplayDate.Month - 1;
            selectedMonthText.Text = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[monthIndex];
            MonthTextBlock.Text = selectedMonthText.Text;

            yearListBox.SelectedItem = SelectedYear;

            calendar.DisplayDate = new DateTime(SelectedYear, monthIndex + 1, 1);
            calendar.SelectedDate = new DateTime(SelectedYear, monthIndex + 1, 1);
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBtn = (Button)sender;
            int selectedMonth = Convert.ToInt32(clickedBtn.Content);

            if (selectedMonth >= 1 && selectedMonth <= 12)
            {
                calendar.DisplayDate = new DateTime(SelectedYear, selectedMonth, 1);
                calendar.SelectedDate = new DateTime(SelectedYear, selectedMonth, 1);

                selectedMonthText.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth);
                MonthTextBlock.Text = selectedMonthText.Text;

            }
        }

        private async void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var userId = App.LoggedInUserId;

            if (calendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = calendar.SelectedDate.Value;
                DateTime noteTime;

                if (DateTime.TryParse(txtTime.Text, out noteTime))
                {
                    DateTime combinedDateTime = selectedDate.Date.Add(noteTime.TimeOfDay);

                    var newNoteDto = new NoteDtoW
                    {
                        Title = txtNote.Text,
                        Time = combinedDateTime,
                        UserId = userId
                    };

                    var response = await _client.PostAsJsonAsync("api/note/addNote", newNoteDto);

                    if (response.IsSuccessStatusCode)
                    {
                        var addedNote = await response.Content.ReadFromJsonAsync<TodoModel>();
                        AllNotes.Add(addedNote);
                        UpdateNotesForSelectedDate(selectedDate);
                        txtNote.Text = null;
                        txtTime.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении заметки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите корректное время в формате чч:мм",
                                    "Ошибка ввода времени",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите дату для заметки на календаре",
                                "Дата не выбрана",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }

        }

        public async void UpdateNotesForSelectedDate(DateTime selectedDate)
        {
            NotesForSelectedDate.Clear();

            try
            {
                var response = await _client.GetAsync($"api/note/getNotesByDate?date={selectedDate:yyyy-MM-dd}");
                response.EnsureSuccessStatusCode();

                var notes = await response.Content.ReadFromJsonAsync<List<TodoModel>>();

                if (notes != null)
                {
                    foreach (var note in notes)
                    {
                        NotesForSelectedDate.Add(note);
                    }
                    NoteCountTextBlock.Text = $"Заметок - {NotesForSelectedDate.Count.ToString()}";
                }
                else
                {
                    MessageBox.Show("No notes found for the selected date.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Request error: {httpEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = calendar.SelectedDate.Value;

                UpdateNotesForSelectedDate(selectedDate);

                DayWeekNameTextBlock.Text = selectedDate.ToString("dddd");

                DayNumberTextBlock.Text = selectedDate.ToString("dd");

                NoteCountTextBlock.Text = $"Заметок - {NotesForSelectedDate.Count.ToString()}";

            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // SaveNotesToDataBase();
            App.LoggedInUserId = 0;

            Application.Current.Shutdown();
        }

        private async void DownloadNotesFromDataBase()
        {
            var userId = App.LoggedInUserId;
            var response = await _client.GetAsync("api/note/getAllNotes?userId=" + userId);
            if (response.IsSuccessStatusCode)
            {
                var notes = await response.Content.ReadFromJsonAsync<List<TodoModel>>();
                foreach (var note in notes)
                {
                    AllNotes.Add(note);
                }
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке заметок", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }
}