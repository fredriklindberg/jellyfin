using Emby.Naming.Common;
using Emby.Naming.TV;
using Xunit;

namespace Jellyfin.Naming.Tests.TV
{
    public class SeriesPathParserTest
    {
        [Theory]
        [InlineData("The.Show.S01", "The.Show", 1)]
        [InlineData("/The.Show.S01", "The.Show", 1)]
        [InlineData("/some/place/The.Show.S01", "The.Show", 1)]
        [InlineData("/something/The.Show.S01", "The.Show", 1)]
        [InlineData("The Show Season 10", "The Show", 10)]
        [InlineData("The Show S01E01", "The Show", 1)]
        [InlineData("The Show S01E01 Episode", "The Show", 1)]
        [InlineData("/something/The Show/Season 1", "The Show", 1)]
        [InlineData("/something/The Show/S01", "The Show", 1)]
        public void SeriesPathParser(string path, string name, int season)
        {
            NamingOptions o = new NamingOptions();
            SeriesPathParser p = new SeriesPathParser(o);
            var res = p.Parse(path);

            Assert.Equal(name, res.SeriesName);
            Assert.Equal(season, res.SeasonNumber);
            Assert.True(res.Success);
        }
    }
}
