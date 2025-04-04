using Microsoft.AspNetCore.Mvc;
using InternIntellegence_MovieApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InternIntellegence_MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavouritesController : ControllerBase
    {
        private readonly MovieService _movieService;

        public FavouritesController(MovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost("favorites")]
        public async Task<IActionResult> AddMovieToFavorites([FromQuery] string userId, [FromQuery] int movieId)
        {
            try
            {
                await _movieService.AddMovieToFavouritesAsync(userId, movieId);
                return Ok(new { message = "Movie added to favorites successfully" });
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while adding the movie to favorites", details = ex.Message });
            }
        }


        [HttpGet("favorites")]
        public async Task<IActionResult> GetUserFavorites([FromQuery] string userId)
        {
            try
            {
                var favoriteMovies = await _movieService.GetUserFavouriteMoviesAsync(userId);
                return Ok(favoriteMovies);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving favorite movies", details = ex.Message });
            }
        }
    }
}

