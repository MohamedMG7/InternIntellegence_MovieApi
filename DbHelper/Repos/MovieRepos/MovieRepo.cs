using AutoMapper;
using InternIntellegence_MovieApi.DTO.MovieDtos;
using InternIntellegence_MovieApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InternIntellegence_MovieApi.DbHelper.Repos.MovieRepos
{
    public class MovieRepo : IMovieRepo
    {
        private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

        public MovieRepo(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
			_mapper = mapper;
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie> UpdateMovieAsync(int id, Movie movieDto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return null;

            
            movie.Title = movieDto.Title;
            movie.Overview = movieDto.Overview;
            movie.ReleaseDate = movieDto.ReleaseDate;
            movie.Rating = movieDto.Rating;
            movie.PosterPath = movieDto.PosterPath;
            movie.BackdropPath = movieDto.BackdropPath;
            movie.Genres = movieDto.Genres;
            movie.Runtime = movieDto.Runtime;

            
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return false;

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }


		#region Searching
		public async Task<IEnumerable<Movie>> SearchMoviesByTitleAsync(string title)
		{
			return await _context.Movies
				.Where(m => m.Title.Contains(title))
				.ToListAsync();
		}

		public async Task<IEnumerable<Movie>> SearchMoviesByRuntimeRangeAsync(int minRuntime, int maxRuntime)
		{
			return await _context.Movies
				.Where(m => m.Runtime >= minRuntime && m.Runtime <= maxRuntime)
				.ToListAsync();
		}

		public async Task<IEnumerable<Movie>> SearchMoviesByRatingRangeAsync(double minRating, double maxRating)
		{
			return await _context.Movies
				.Where(m => m.Rating >= minRating && m.Rating <= maxRating)
				.ToListAsync();
		}

		public async Task<IEnumerable<Movie>> SearchMoviesByGenreAsync(string genre)
		{
			return await _context.Movies
				.Where(m => m.Genres.Contains(genre))
				.ToListAsync();
		}

		public async Task<IEnumerable<Movie>> SearchMoviesByReleaseYearAsync(int year)
		{
			return await _context.Movies
				.Where(m => m.ReleaseDate.StartsWith(year.ToString()))
				.ToListAsync();
		}

		public async Task<IEnumerable<Movie>> AdvancedSearchAsync(
			string title = null,
			int? minRuntime = null,
			int? maxRuntime = null,
			double? minRating = null,
			double? maxRating = null,
			string genre = null,
			int? year = null)
		{
			var query = _context.Movies.AsQueryable();

			if (!string.IsNullOrEmpty(title))
			{
				query = query.Where(m => m.Title.Contains(title));
			}

			if (minRuntime.HasValue)
			{
				query = query.Where(m => m.Runtime >= minRuntime.Value);
			}

			if (maxRuntime.HasValue)
			{
				query = query.Where(m => m.Runtime <= maxRuntime.Value);
			}

			if (minRating.HasValue)
			{
				query = query.Where(m => m.Rating >= minRating.Value);
			}

			if (maxRating.HasValue)
			{
				query = query.Where(m => m.Rating <= maxRating.Value);
			}

			if (!string.IsNullOrEmpty(genre))
			{
				query = query.Where(m => m.Genres.Contains(genre));
			}

			if (year.HasValue)
			{
				query = query.Where(m => m.ReleaseDate.StartsWith(year.Value.ToString()));
			}

			return await query.ToListAsync();
		}
		#endregion

		public async Task<List<int>> GetUserFavouriteMoviesAsync(string userId)
		{
			return await _context.UserFavourites
				.Where(uf => uf.UserId == userId)
				.Select(uf => uf.MovieId)
				.ToListAsync();
		}
    }
}
