using AutoMapper;
using InternIntellegence_MovieApi.DbHelper;
using InternIntellegence_MovieApi.DbHelper.Repos.MovieRepos;
using InternIntellegence_MovieApi.DTO.MovieDtos;
using InternIntellegence_MovieApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InternIntellegence_MovieApi.Services
{
	public class MovieService
	{
		private readonly IMovieRepo _movieRepo;
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;
		

		public MovieService(IMovieRepo movieRepo,IMapper mapper,UserManager<ApplicationUser> userManager)
		{
			_movieRepo = movieRepo;
			_mapper = mapper;
			_userManager = userManager;
			
		}

		public async Task<Movie> CreateMovieAsync(MovieAddDto movie)
		{
			var movieEntity = _mapper.Map<Movie>(movie);
			return await _movieRepo.CreateMovieAsync(movieEntity);
		}

		public async Task<MovieReadDto?> GetMovieByIdAsync(int id)
		{
			var movie = await _movieRepo.GetMovieByIdAsync(id);
			return movie != null ? _mapper.Map<MovieReadDto>(movie) : null;
		}

		public async Task<IEnumerable<MovieReadDto>> GetAllMoviesAsync()
		{
			var movies = await _movieRepo.GetAllMoviesAsync();
			return _mapper.Map<IEnumerable<MovieReadDto>>(movies);
		}

		public async Task UpdateMovieAsync(int id, MovieUpdateDto movieDto)
		{
			var movieEntity = _mapper.Map<Movie>(movieDto);
			await _movieRepo.UpdateMovieAsync(id, movieEntity);
		}

		public async Task DeleteMovieAsync(int id)
		{
			await _movieRepo.DeleteMovieAsync(id);
		}

		#region Searching Movies
		public async Task<IEnumerable<MovieReadDto>> SearchMoviesByTitleAsync(string title)
		{
			var movies = await _movieRepo.SearchMoviesByTitleAsync(title);
			return _mapper.Map<IEnumerable<MovieReadDto>>(movies);
		}

		public async Task<IEnumerable<MovieReadDto>> SearchMoviesByRuntimeRangeAsync(int minRuntime, int maxRuntime)
		{
			var movies = await _movieRepo.SearchMoviesByRuntimeRangeAsync(minRuntime, maxRuntime);
			return _mapper.Map<IEnumerable<MovieReadDto>>(movies);
		}

		public async Task<IEnumerable<MovieReadDto>> SearchMoviesByRatingRangeAsync(double minRating, double maxRating)
		{
			var movies = await _movieRepo.SearchMoviesByRatingRangeAsync(minRating, maxRating);
			return _mapper.Map<IEnumerable<MovieReadDto>>(movies);
		}

		public async Task<IEnumerable<MovieReadDto>> AdvancedSearchAsync(
			string title = null,
			int? minRuntime = null,
			int? maxRuntime = null,
			double? minRating = null,
			double? maxRating = null,
			string genre = null,
			int? year = null)
		{
			var movies = await _movieRepo.AdvancedSearchAsync(
				title,
				minRuntime,
				maxRuntime,
				minRating,
				maxRating,
				genre,
				year);
			return _mapper.Map<IEnumerable<MovieReadDto>>(movies);
		}
		#endregion

		#region Favourites Management
		public async Task AddMovieToFavouritesAsync(string userId, int movieId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new ApplicationException("User not found");
			}

			var movie = await _movieRepo.GetMovieByIdAsync(movieId);
			if (movie == null)
			{
				throw new ApplicationException("Movie not found");
			}

			var favourite = new UserFavourites
			{
				UserId = userId,
				MovieId = movieId
			};

			user.Favourites.Add(favourite);
			await _userManager.UpdateAsync(user);
		}

		public async Task<IEnumerable<MovieReadDto>> GetUserFavouriteMoviesAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new ApplicationException("User not found");
			}

			var moviedIds = await _movieRepo.GetUserFavouriteMoviesAsync(userId);
			
			var favouriteMovies = new List<MovieReadDto>();
			foreach(var movieId in moviedIds){
				var movie = await GetMovieByIdAsync(movieId);
				if (movie != null)
				{
					favouriteMovies.Add(movie);
				}
			}

			return favouriteMovies;
		}
		#endregion
	}
}
