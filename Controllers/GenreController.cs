using Microsoft.AspNetCore.Mvc;

namespace InternIntellegence_MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly GenreService _genreService;

        public GenreController(GenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpPost("update-from-tmdb")]
        public async Task<IActionResult> UpdateGenresFromTmdb()
        {
            try
            {
                await _genreService.UpdateGenresFromTmdbAsync();
                return Ok(new { message = "Genres updated successfully from TMDB" });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    new { message = "Error connecting to TMDB API", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while updating genres", details = ex.Message });
            }
        }
    }
}