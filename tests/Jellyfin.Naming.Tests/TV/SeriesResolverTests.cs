using Emby.Naming.Common;
using Emby.Naming.TV;
using Xunit;

namespace Jellyfin.Naming.Tests.TV
{
    public class SeriesResolverTests
    {
        [Theory]
        [InlineData("The.Show.S01", "The Show", 1)]
        [InlineData("The.Show.S01.COMPLETE", "The Show", 1)]
        [InlineData("S.H.O.W.S01", "S.H.O.W", 1)]
        [InlineData("The.Show.P.I.S01", "The Show P.I", 1)]
        [InlineData("The_Show_Season_1", "The Show", 1)]
        [InlineData("/something/The_Show/Season 10", "The Show", 10)]
        [InlineData("The Show", "The Show", null)]
        [InlineData("/some/path/The Show", "The Show", null)]
        public void SeriesResolverTest(string path, string name, int? season)
        {
            NamingOptions o = new NamingOptions();
            SeriesResolver resolver = new SeriesResolver(o);
            var res = resolver.Resolve(path);

            Assert.Equal(name, res.Name);
            Assert.Equal(season, res.SeasonNumber);
        }
    }
}
