using Xunit;

namespace ContreJourDX.Browser.Platform.Tests
{
    public class BrowserLanguageTests
    {
        [Theory]
        [InlineData("?lang=ru", "en-US", "ru")]
        [InlineData("?x=1&lang=JA&y=2", "en-US", "ja")]
        [InlineData("", "zh-TW", "zh")]
        [InlineData("", "zh-Hant-TW", "zh")]
        [InlineData("", "pt_BR", "pt")]
        [InlineData("?lang=", "de", "de")]
        [InlineData("?lang=xyz", "fr-FR", "fr")]
        [InlineData("?lang=1a", "", "en")]
        [InlineData("", "", "en")]
        [InlineData(null, null, "en")]
        [InlineData("?lang=xx", "ru", "xx")]
        public void QueryWinsThenNavigatorThenEnglish(string query, string navigator, string expected)
        {
            Assert.Equal(expected, BrowserLanguage.Parse(query, navigator));
        }
    }
}
