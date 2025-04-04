namespace InternIntellegence_MovieApi.DTO.MovieDtos{
    public class MovieFilterDto{
        public string? Title { get; set; }
        public List<string>? Genres { get; set; }
        public int? MinRuntime { get; set; }
        public int? MaxRuntime { get; set; }
        public double? MinRating { get; set; }
        public double? MaxRating { get; set; }
    }
}