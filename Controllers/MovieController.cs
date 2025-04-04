using InternIntellegence_MovieApi.DTO.MovieDtos;
using InternIntellegence_MovieApi.Models;
using InternIntellegence_MovieApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternIntellegence_MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly TMDBService _tmdbService;
        private readonly MovieService _movieService;

        public MovieController(TMDBService tmdbService,MovieService movieService)
        {
            _tmdbService = tmdbService;
            _movieService = movieService;
        }

        

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieAddDto movieDto)
        {
            try
            {
                var createdMovie = await _movieService.CreateMovieAsync(movieDto);
                return CreatedAtAction(nameof(GetMovieById), new { id = createdMovie.Id }, createdMovie);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error creating movie", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            try
            {
                var movie = await _movieService.GetMovieByIdAsync(id);
                if (movie == null)
                {
                    return NotFound();
                }
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error retrieving movie", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error retrieving movies", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieUpdateDto movieDto)
        {
            try
            {
                await _movieService.UpdateMovieAsync(id, movieDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error updating movie", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                await _movieService.DeleteMovieAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error deleting movie", details = ex.Message });
            }
        }


        #region Searching
        [HttpGet("search/title")]
        public async Task<IActionResult> SearchByTitle([FromQuery] string title)
        {
            try
            {
                var movies = await _movieService.SearchMoviesByTitleAsync(title);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while searching movies", details = ex.Message });
            }
        }

        [HttpGet("search/runtime")]
        public async Task<IActionResult> SearchByRuntimeRange(
            [FromQuery] int minRuntime, 
            [FromQuery] int maxRuntime)
        {
            try
            {
                var movies = await _movieService.SearchMoviesByRuntimeRangeAsync(minRuntime, maxRuntime);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while searching movies", details = ex.Message });
            }
        }

        [HttpGet("search/rating")]
        public async Task<IActionResult> SearchByRatingRange(
            [FromQuery] double minRating, 
            [FromQuery] double maxRating)
        {
            try
            {
                var movies = await _movieService.SearchMoviesByRatingRangeAsync(minRating, maxRating);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while searching movies", details = ex.Message });
            }
        }

        [HttpGet("search/advanced")]
        public async Task<IActionResult> AdvancedSearch(
            [FromQuery] string title = null,
            [FromQuery] int? minRuntime = null,
            [FromQuery] int? maxRuntime = null,
            [FromQuery] double? minRating = null,
            [FromQuery] double? maxRating = null,
            [FromQuery] string genre = null,
            [FromQuery] int? year = null)
        {
            try
            {
                var movies = await _movieService.AdvancedSearchAsync(
                    title,
                    minRuntime,
                    maxRuntime,
                    minRating,
                    maxRating,
                    genre,
                    year);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while searching movies", details = ex.Message });
            }
        }
        #endregion
    }
}
