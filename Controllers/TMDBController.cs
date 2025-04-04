using InternIntellegence_MovieApi.DTO.MovieDtos;
using InternIntellegence_MovieApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace InternIntellegence_MovieApi.Controllers{
    
    [ApiController]
    [Route("api/[controller]")]
    public class TMDBController : ControllerBase
    {
        private readonly TMDBService _tmdbService;

        public TMDBController(TMDBService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieFromTMDBById(int id)
        {
            try
            {
                var movieData = await _tmdbService.getMovieById(id);
                return Ok(movieData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error fetching movie data from TMDB", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpPost("fetch-and-store/{id}")]
        public async Task<IActionResult> FetchAndStoreMovie(int id)
        {
            try
            {
                await _tmdbService.FetchAndStoreMovieById(id);
                return Ok(new { message = "Movie data successfully fetched and stored" });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error fetching movie data from TMDB", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An unexpected error occurred", details = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] MovieFilterDto filter)
        {
            try
            {
                var movies = await _tmdbService.SearchMoviesAsync(filter);
                return Ok(movies);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    new { message = "Error connecting to TMDB API", details = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = ex.Message, details = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An unexpected error occurred", details = ex.Message });
            }
        }
    }
}
