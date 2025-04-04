using System.Text;

namespace InternIntellegence_MovieApi.Services{
    public class TMDBMovieFilterBuilder
    {
        private readonly StringBuilder _queryBuilder = new StringBuilder();
        private bool _hasFilters = false;

        public TMDBMovieFilterBuilder WithTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                AddFilter($"title={Uri.EscapeDataString(title)}");
            }
            return this;
        }

        public TMDBMovieFilterBuilder WithGenres(List<string> genres)
        {
            if (genres != null && genres.Any())
            {
                AddFilter($"with_genres={string.Join(",", genres)}");
            }
            return this;
        }

        public TMDBMovieFilterBuilder WithRuntime(int? minRuntime, int? maxRuntime)
        {
            if (minRuntime.HasValue || maxRuntime.HasValue)
            {
                var runtimeFilter = string.Empty;
                if (minRuntime.HasValue && maxRuntime.HasValue)
                {
                    runtimeFilter = $"with_runtime.gte={minRuntime}&with_runtime.lte={maxRuntime}";
                }
                else if (minRuntime.HasValue)
                {
                    runtimeFilter = $"with_runtime.gte={minRuntime}";
                }
                else if (maxRuntime.HasValue)
                {
                    runtimeFilter = $"with_runtime.lte={maxRuntime}";
                }
                AddFilter(runtimeFilter);
            }
            return this;
        }

        public TMDBMovieFilterBuilder WithRating(double? minRating, double? maxRating)
        {
            if (minRating.HasValue || maxRating.HasValue)
            {
                var ratingFilter = string.Empty;
                if (minRating.HasValue && maxRating.HasValue)
                {
                    ratingFilter = $"vote_average.gte={minRating}&vote_average.lte={maxRating}";
                }
                else if (minRating.HasValue)
                {
                    ratingFilter = $"vote_average.gte={minRating}";
                }
                else if (maxRating.HasValue)
                {
                    ratingFilter = $"vote_average.lte={maxRating}";
                }
                AddFilter(ratingFilter);
            }
            return this;
        }

        public string Build()
        {
            return _queryBuilder.ToString();
        }

        private void AddFilter(string filter)
        {
            if (_hasFilters)
            {
                _queryBuilder.Append('&');
            }
            else
            {
                _hasFilters = true;
            }
            _queryBuilder.Append(filter);
        }
    }
}