using System.Globalization;
using Emby.Naming.Common;

namespace Emby.Naming.TV
{
    /// <summary>
    /// Used to parse information about series from paths containing more information that only the series name.
    /// Uses the same regular expressions as the EpisodePathParser but have different success criteria.
    /// </summary>
    public class SeriesPathParser
    {
        private readonly NamingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesPathParser"/> class.
        /// </summary>
        /// <param name="options"><see cref="NamingOptions"/> object containing EpisodeExpressions and MultipleEpisodeExpressions.</param>
        public SeriesPathParser(NamingOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Parses information about series from path.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>Returns <see cref="SeriesPathParserResult"/> object.</returns>
        public SeriesPathParserResult Parse(string path)
        {
            SeriesPathParserResult? result = null;

            foreach (var expression in _options.EpisodeExpressions)
            {
                var currentResult = Parse(path, expression);
                if (currentResult.Success)
                {
                    result = currentResult;
                    break;
                }
            }

            if (result != null)
            {
                if (!string.IsNullOrEmpty(result.SeriesName))
                {
                    result.SeriesName = result.SeriesName
                        .Trim()
                        .Trim('_', '.', '-')
                        .Trim();
                }
            }

            return result ?? new SeriesPathParserResult();
        }

        private static SeriesPathParserResult Parse(string name, EpisodeExpression expression)
        {
            var result = new SeriesPathParserResult();

            var match = expression.Regex.Match(name);

            if (match.Success && match.Groups.Count >= 3)
            {
                if (expression.IsNamed)
                {
                    if (int.TryParse(match.Groups["seasonnumber"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var num))
                    {
                        result.SeasonNumber = num;
                    }

                    result.SeriesName = match.Groups["seriesname"].Value;
                    result.Success = !string.IsNullOrEmpty(result.SeriesName) && result.SeasonNumber != null;
                }
                else
                {
                    if (int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var num))
                    {
                        result.SeasonNumber = num;
                    }
                }

                // Invalidate match when the season is 200 through 1927 or above 2500
                // because it is an error unless the TV show is intentionally using false season numbers.
                // It avoids erroneous parsing of something like "Series Special (1920x1080).mkv" as being season 1920 episode 1080.
                if ((result.SeasonNumber >= 200 && result.SeasonNumber < 1928)
                    || result.SeasonNumber > 2500)
                {
                    result.Success = false;
                }
            }

            return result;
        }
    }
}
