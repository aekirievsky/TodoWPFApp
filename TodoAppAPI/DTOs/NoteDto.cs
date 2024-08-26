namespace TodoAppAPI.DTOs
{
    public class NoteDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public int UserId { get; set; }
    }
}
