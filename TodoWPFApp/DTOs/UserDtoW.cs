
namespace TodoWPFApp.DTOs
{
    public class UserDtoW
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public IEnumerable<NoteDtoW> Notes { get; set; }
    }
}
