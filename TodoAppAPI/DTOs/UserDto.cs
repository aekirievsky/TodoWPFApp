namespace TodoAppAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public IEnumerable<NoteDto> Notes { get; set; }
    }
}
