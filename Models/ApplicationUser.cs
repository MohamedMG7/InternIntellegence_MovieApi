using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace InternIntellegence_MovieApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserFavourites> Favourites { get; set; } = new List<UserFavourites>();
    }
}
