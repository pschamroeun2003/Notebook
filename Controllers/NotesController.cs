using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using NoteTakingApp.DTOs;
using NoteTakingApp.Models;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace NoteTakingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class NotesController : ControllerBase
    {
        private readonly string _connectionString;

        public NotesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

      
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<NoteDTO>>>> GetNotes(bool? active = null)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Status = "Error",
                        Data = "User is not authenticated."
                    });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT Id, Title, Content, CreatedAt, UpdatedAt, Status FROM Notes WHERE UserId = @UserId";
                    if (active.HasValue)
                    {
                        query += " AND Status = @Active";
                    }

                    var notes = await connection.QueryAsync<NoteDTO>(query, new { UserId = userId, Active = active });

                    var response = new ApiResponse<IEnumerable<NoteDTO>>
                    {
                        Status = "Success",
                        Data = notes
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching notes: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Status = "Error",
                    Data = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<NoteDTO>>> CreateNote([FromBody] NoteDTO noteDto)
        {
            if (noteDto == null)
            {
                return BadRequest(new ApiResponse<string> { Status = "Error", Data = "Note data is required." });
            }

            var userId = GetUserId();
            var note = new Note
            {
                Title = noteDto.Title,
                Content = noteDto.Content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = userId,
                Status = noteDto.Status ?? true 
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "INSERT INTO Notes (Title, Content, CreatedAt, UpdatedAt, UserId, Status) " +
                            "VALUES (@Title, @Content, @CreatedAt, @UpdatedAt, @UserId, @Status); " +
                            "SELECT CAST(SCOPE_IDENTITY() as int)";

                var noteId = await connection.QuerySingleAsync<int>(query, note);

                note.Id = noteId;

                var noteToReturn = new NoteDTO
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt,
                    UpdatedAt = note.UpdatedAt,
                    Status = note.Status
                };

                var response = new ApiResponse<NoteDTO>
                {
                    Status = "Success",
                    Data = noteToReturn
                };

                return CreatedAtAction(nameof(GetNotes), new { id = noteToReturn.Id }, response);
            }
        }

    [HttpPut("{id}")]
   public async Task<ActionResult<ApiResponse<NoteDTO>>> UpdateNote(int id, [FromBody] NoteDTO noteDto)
        {
            if (noteDto == null)
            {
                return BadRequest(new ApiResponse<string> { Status = "Error", Data = "Note data is required." });
            }

            var userId = GetUserId();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT Id, Title, Content, CreatedAt, UpdatedAt, Status FROM Notes WHERE Id = @Id AND UserId = @UserId";
                var note = await connection.QueryFirstOrDefaultAsync<Note>(query, new { Id = id, UserId = userId });
                if (note == null)
                {
                    return NotFound(new ApiResponse<string> { Status = "Error", Data = "Note not found or you don't have permission to modify it." });
                }
                note.Title = noteDto.Title;
                note.Content = noteDto.Content;
                note.UpdatedAt = DateTime.Now;
                note.Status = noteDto.Status ?? note.Status;

                var updateQuery = "UPDATE Notes SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt, Status = @Status WHERE Id = @Id AND UserId = @UserId";
                await connection.ExecuteAsync(updateQuery, new { note.Title, note.Content, note.UpdatedAt, note.Status, note.Id, UserId = userId });

                var updatedNote = new NoteDTO
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt,
                    UpdatedAt = note.UpdatedAt,
                    Status = note.Status
                };

                var response = new ApiResponse<NoteDTO>
                {
                    Status = "Success",
                    Data = updatedNote
                };

                return Ok(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteNote(int id)
        {
            var userId = GetUserId();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT Id FROM Notes WHERE Id = @Id AND UserId = @UserId";
                var note = await connection.QueryFirstOrDefaultAsync<Note>(query, new { Id = id, UserId = userId });

                if (note == null)
                {
                    return NotFound(new ApiResponse<string> { Status = "Error", Data = "Note not found or you don't have permission to delete it." });
                }

                var deleteQuery = "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId";
                await connection.ExecuteAsync(deleteQuery, new { Id = id, UserId = userId });

                var response = new ApiResponse<string>
                {
                    Status = "Success",
                    Data = "Note deleted successfully."
                };

                return Ok(response);
            }
        }
    }
}
