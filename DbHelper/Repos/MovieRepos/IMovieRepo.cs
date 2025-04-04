using InternIntellegence_MovieApi.DTO.MovieDtos;
using InternIntellegence_MovieApi.Models;

namespace InternIntellegence_MovieApi.DbHelper.Repos.MovieRepos
{
    public interface IMovieRepo
    {
        Task<Movie> CreateMovieAsync(Movie movieDto);
        Task<Movie> GetMovieByIdAsync(int id);
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<Movie> UpdateMovieAsync(int id, Movie movieDto);
        Task<bool> DeleteMovieAsync(int id);

		#region Searching
		Task<IEnumerable<Movie>> SearchMoviesByTitleAsync(string title);
		Task<IEnumerable<Movie>> SearchMoviesByRuntimeRangeAsync(int minRuntime, int maxRuntime);
		Task<IEnumerable<Movie>> SearchMoviesByRatingRangeAsync(double minRating, double maxRating);
		Task<IEnumerable<Movie>> AdvancedSearchAsync(
			string title = null,
			int? minRuntime = null,
			int? maxRuntime = null,
			double? minRating = null,
			double? maxRating = null,
			string genre = null,
			int? year = null);
		#endregion

		Task<List<int>> GetUserFavouriteMoviesAsync(string userId);
    }
}
