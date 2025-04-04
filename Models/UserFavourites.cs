namespace InternIntellegence_MovieApi.Models
{
    public class UserFavourites
    {
        public ApplicationUser User { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public Movie movie { get; set; } = null!;
        public int MovieId { get; set; }
    }
}
