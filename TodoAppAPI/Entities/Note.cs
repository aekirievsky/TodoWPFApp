namespace TodoAppAPI.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
