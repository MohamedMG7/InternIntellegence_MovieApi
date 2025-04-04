namespace InternIntellegence_MovieApi.DTO.MovieDtos
{
    public class MovieUpdateDto
    {
        public string Title { get; set; } = null!;
        public string Overview { get; set; } = null!; 
        public string ReleaseDate { get; set; } = null!; 
        public double Rating { get; set; } 
        public string PosterPath { get; set; } = null!; 
        public string BackdropPath { get; set; } = null!; 
        public List<string> Genres { get; set; } = new List<string>(); 
        public int Runtime { get; set; }
    }
}
