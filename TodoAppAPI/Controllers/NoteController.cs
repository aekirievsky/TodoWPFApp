using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAppAPI.Data;
using TodoAppAPI.DTOs;
using TodoAppAPI.Entities;

namespace TodoAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NoteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("addNote")]
        public async Task<IActionResult> AddNote([FromBody] NoteDto noteDto)
        {
            if (noteDto == null)
            {
                return BadRequest("NoteDto cannot be null");
            }

            try
            {

                var user = await _context.Users.FindAsync(noteDto.UserId);
                if (user == null)
                {
                    return NotFound("User not found");
                }


                var note = new Note
                {
                    Title = noteDto.Title,
                    Time = noteDto.Time,
                    UserId = noteDto.UserId
                };


                _context.Notes.Add(note);
                await _context.SaveChangesAsync();


                var createdNoteDto = new NoteDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Time = note.Time,
                    UserId = note.UserId
                };

                return Ok(createdNoteDto);

            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Database error occurred: " + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest("Argument null error: " + ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest("Invalid format: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getNoteById")]
        public async Task<IActionResult> GetNoteById([FromQuery] int Id)
        {
            var note = await _context.Notes.FindAsync(Id);
            if (note == null)
            {
                return NotFound("Note not found");
            }

            var noteResponse = new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Time = note.Time,
                UserId = note.UserId
            };

            return Ok(noteResponse);
        }

        [HttpGet("getAllNotes")]
        public async Task<IActionResult> GetAllNotes([FromQuery] int userId)
        {
            var user = await _context.Users.Include(u => u.Notes)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var notesList = user.Notes.Select(n => new NoteDto
            {
                Id = n.Id,
                Title = n.Title,
                Time = n.Time
            }).ToList();

            return Ok(notesList);
        }

        [HttpPut("editNote")]
        public async Task<IActionResult> EditNote([FromBody] NoteDto noteDto)
        {
            if (noteDto == null)
            {
                return BadRequest("NoteDto cannot be null");
            }

            var user = await _context.Users.FindAsync(noteDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var note = await _context.Notes.FindAsync(noteDto.Id);
            if (note == null)
            {
                return NotFound("Note not found");
            }

            note.Title = noteDto.Title;
            note.Time = noteDto.Time;

            await _context.SaveChangesAsync();

            return Ok(note);
        }

        [HttpDelete("deleteNote")]
        public async Task<IActionResult> DeleteNote([FromQuery] int Id)
        {
            var note = await _context.Notes.FindAsync(Id);
            if (note == null)
            {
                return NotFound("Note not found");
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return Ok("Note deleted successfully");
        }
    }
}
