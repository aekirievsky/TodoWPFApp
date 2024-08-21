

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TodoWPFApp.Models
{
    public class TodoModel : INotifyPropertyChanged
    {
        private int _Id;
        [Key]        
        
        public int Id
        {
            get => _Id;
            set => SetField(ref _Id, value);
        }

        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        private DateTime _time;
        public DateTime Time
        {
            get => _time;
            set => SetField(ref _time, value);
        }

       


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
