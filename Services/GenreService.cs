using InternIntellegence_MovieApi.DbHelper;
using InternIntellegence_MovieApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

public class GenreService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;
    private readonly string _apiKey;

    public GenreService(HttpClient httpClient, ApplicationDbContext context, IConfiguration config)
    {
        _httpClient = httpClient;
        _context = context;
        _apiKey = config["TMDB:ApiKey"];
    }

    public async Task UpdateGenresFromTmdbAsync()
    {
        try
        {
            var url = $"https://api.themoviedb.org/3/genre/movie/list";
            
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            using var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"TMDB API request failed: {response.StatusCode}, {errorContent}");
            }

            var responseData = await response.Content.ReadFromJsonAsync<TmdbGenreResponse>();

            foreach (var genreDto in responseData.Genres)
            {
                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.Id == genreDto.Id);

                if (existingGenre == null)
                {
                    _context.Genres.Add(new Genre
                    {
                        Id = genreDto.Id,
                        name = genreDto.Name
                    });
                }
                else if (existingGenre.name != genreDto.Name)
                {
                    existingGenre.name = genreDto.Name;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "No inner exception";
            throw new Exception($"Database update failed: {innerMessage}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred", ex);
        }
    }

    private class TmdbGenreResponse
    {
        public List<TmdbGenreDto> Genres { get; set; }
    }

    private class TmdbGenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}