

namespace TodoWPFApp.Models
{
    public class TodoModel
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public bool IsDelete { get; set; }
        public bool IsEdit { get; set; }
        public bool IsCompleted { get; set; }
    }
}
