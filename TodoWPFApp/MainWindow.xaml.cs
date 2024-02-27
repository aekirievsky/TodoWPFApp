using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoWPFApp.Data.DataBase;


namespace TodoWPFApp
{
    public partial class MainWindow : Window
    {
        public List<int> Years { get; set; }
        public int SelectedYear { get; set; }

        public readonly AppDbContext appDbContext;

        public MainWindow()
        {
            InitializeComponent();

            appDbContext = new AppDbContext();

            Years = new List<int>();

            for (int year = 2020; year <= 2027; year++)
            {
                Years.Add(year);
            }

            SelectedYear = DateTime.Now.Year;
            DateTime startDate = new DateTime(SelectedYear, 1, 1);
            calendar.DisplayDate = startDate;
            calendar.SelectedDate = startDate;

            UpdateCalendar();
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


        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = calendar.SelectedDate.Value;

                DayWeekNameTextBlock.Text = selectedDate.ToString("dddd");

                DayNumberTextBlock.Text = selectedDate.ToString("dd");
            }
        }
    }
}