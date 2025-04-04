namespace InternIntellegence_MovieApi.Models
{
	public class Movie
	{
		public int Id { get; set; } // TMDB Movie ID
		public string Title { get; set; } = null!; // Movie title
		public string Overview { get; set; } = null!; // Movie description
		public string ReleaseDate { get; set; } = null!; // Release date (YYYY-MM-DD)
		public double Rating { get; set; } // TMDB average rating
		public string PosterPath { get; set; } = null!; // Poster image URL
		public string BackdropPath { get; set; } = null!; // Background image URL
		public List<string> Genres { get; set; } = new List<string>(); // List of genre names
		public int Runtime { get; set; } // Movie duration in minutes

		public ICollection<UserFavourites> FavouritedBy { get; set; } = new List<UserFavourites>();

	}
}
