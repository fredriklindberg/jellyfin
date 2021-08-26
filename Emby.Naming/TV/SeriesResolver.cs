using System.IO;
using System.Text.RegularExpressions;
using Emby.Naming.Common;

namespace Emby.Naming.TV
{
    /// <summary>
    /// Used to resolve information about series from path.
    /// </summary>
    public class SeriesResolver
    {
        private SeriesPathParser seriesPathParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesResolver"/> class.
        /// </summary>
        /// <param name="options"><see cref="NamingOptions"/> object passed to <see cref="SeriesPathParser"/>.</param>
        public SeriesResolver(NamingOptions options)
        {
            seriesPathParser = new SeriesPathParser(options);
        }

        /// <summary>
        /// Resolve information about series from path.
        /// </summary>
        /// <param name="path">Path to series.</param>
        /// <returns>SeriesInfo.</returns>
        public SeriesInfo Resolve(string path)
        {
            string seriesName = Path.GetFileName(path);
            int? seasonNumber = null;

            SeriesPathParserResult result = seriesPathParser.Parse(path);
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(result.SeriesName))
                {
                    seriesName = result.SeriesName;
                }

                if (result.SeasonNumber != null)
                {
                    seasonNumber = result.SeasonNumber;
                }
            }

            if (!string.IsNullOrEmpty(seriesName))
            {
                seriesName = Regex.Replace(seriesName, @"((?<a>[^\._]{2,})[\._]*)|([\._](?<b>[^\._]{2,}))", "${a} ${b}").Trim();
            }

            return new SeriesInfo(path)
            {
                Name = seriesName,
                SeasonNumber = seasonNumber
            };
        }
    }
}
