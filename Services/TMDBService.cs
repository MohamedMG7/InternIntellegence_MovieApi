using AutoMapper;
using InternIntellegence_MovieApi.DbHelper.Repos.MovieRepos;
using InternIntellegence_MovieApi.DTO.MovieDtos;
using InternIntellegence_MovieApi.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InternIntellegence_MovieApi.Services
{

	
	public class TMDBService
	{
		private readonly IMovieRepo _movieRepo;
		private readonly HttpClient _httpClient;
		private readonly IMapper _mapper;

		public TMDBService(IMovieRepo movieRepo, HttpClient httpClient, IMapper mapper)
		{
			_movieRepo = movieRepo;
			_httpClient = httpClient;
			_mapper = mapper;
		}

		//string request
		private readonly string api_jwt_token = "Put_Your_TMDB_Token_Here";
		private const string BaseUrl = "https://api.themoviedb.org/3/";

		public async Task<MovieReadDto> getMovieById(int movieId){
			string url = $"{BaseUrl}movie/{movieId}";

			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", api_jwt_token);

			using var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var jsonElement = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

			var jsonString = await response.Content.ReadAsStringAsync();
			return new MovieReadDto
			{
				Id = jsonElement.GetProperty("id").GetInt32(),
				Title = jsonElement.GetProperty("title").GetString()!,
				Overview = jsonElement.GetProperty("overview").GetString()!,
				ReleaseDate = jsonElement.GetProperty("release_date").GetString()!,
				Rating = jsonElement.GetProperty("vote_average").GetDouble(),
				PosterPath = jsonElement.GetProperty("poster_path").GetString()!,
				BackdropPath = jsonElement.GetProperty("backdrop_path").GetString()!,
				Genres = jsonElement.GetProperty("genres")
					.EnumerateArray()
					.Select(g => g.GetProperty("name").GetString()!)
					.ToList(),
				Runtime = jsonElement.GetProperty("runtime").GetInt32()
			};
		}

		public async Task FetchAndStoreMovieById(int movieId)
		{
			try
			{
				// Step 1: Fetch movie data from TMDB API
				var movieData = await GetMovieDataFromApi(movieId);
				
				// Step 2: Map API data to our database model (without ID)
				var movieEntity = MapApiDataToMovieEntity(movieData);
				
				// Step 3: Store in database
				await StoreMovieInDatabase(movieEntity);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error fetching and storing movie data", ex);
			}
		}

		private async Task<JsonElement> GetMovieDataFromApi(int movieId)
		{
			string url = $"{BaseUrl}movie/{movieId}";
			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", api_jwt_token);

			using var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
		}

		private Movie MapApiDataToMovieEntity(JsonElement jsonElement)
		{
			return new Movie
			{
				Title = jsonElement.GetProperty("title").GetString() ?? "",
				Overview = jsonElement.GetProperty("overview").GetString() ?? "",
				ReleaseDate = jsonElement.GetProperty("release_date").GetString() ?? "",
				Rating = jsonElement.GetProperty("vote_average").GetDouble(),
				PosterPath = jsonElement.GetProperty("poster_path").GetString() ?? "",
				BackdropPath = jsonElement.GetProperty("backdrop_path").GetString() ?? "",
				Genres = jsonElement.TryGetProperty("genres", out var genresArray)
					? genresArray.EnumerateArray().Select(g => g.GetProperty("name").GetString() ?? "").ToList()
					: new List<string>(),
				Runtime = jsonElement.TryGetProperty("runtime", out var runtime) ? runtime.GetInt32() : 0
			};
		}


		private async Task StoreMovieInDatabase(Movie movie)
		{
			await _movieRepo.CreateMovieAsync(movie);
		}

		public async Task<List<MovieReadDto>> SearchMoviesAsync(MovieFilterDto filter)
		{
			try
			{
				var filterBuilder = new TMDBMovieFilterBuilder()
					.WithTitle(filter.Title)
					.WithGenres(filter.Genres)
					.WithRuntime(filter.MinRuntime, filter.MaxRuntime)
					.WithRating(filter.MinRating, filter.MaxRating);

				var url = $"{BaseUrl}discover/movie?{filterBuilder.Build()}";

				using var request = new HttpRequestMessage(HttpMethod.Get, url);
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", api_jwt_token);

				using var response = await _httpClient.SendAsync(request);
				response.EnsureSuccessStatusCode();

				var jsonElement = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
				var movies = jsonElement.GetProperty("results").EnumerateArray()
					.Select(m => new MovieReadDto
					{
						Id = m.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
						Title = m.TryGetProperty("title", out var title) ? title.GetString() : string.Empty,
						Overview = m.TryGetProperty("overview", out var overview) ? overview.GetString() : string.Empty,
						ReleaseDate = m.TryGetProperty("release_date", out var releaseDate) ? releaseDate.GetString() : string.Empty,
						Rating = m.TryGetProperty("vote_average", out var rating) ? rating.GetDouble() : 0,
						PosterPath = m.TryGetProperty("poster_path", out var posterPath) ? posterPath.GetString() : string.Empty,
						BackdropPath = m.TryGetProperty("backdrop_path", out var backdropPath) ? backdropPath.GetString() : string.Empty,
						Genres = m.TryGetProperty("genre_ids", out var genreIds) 
							? genreIds.EnumerateArray().Select(g => g.GetInt32().ToString()).ToList()
							: new List<string>(),
						Runtime = m.TryGetProperty("runtime", out var runtime) ? runtime.GetInt32() : 0
					})
					.Where(m => m.Id != 0) // Filter out invalid entries
					.ToList();

				return movies;
			}
			catch (Exception ex)
			{
				// Log the error
				throw new ApplicationException("Error processing movie search results", ex);
			}
		}
	}
}
