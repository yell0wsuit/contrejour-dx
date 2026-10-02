using System.Globalization;

using Mokus2D.Localization;

using Xunit;

namespace Mokus2D.Tests
{
    public class LocalizationBundleTests
    {
        [Fact]
        public void OverrideWinsOverTheCulture()
        {
            try
            {
                LocalizationBundle.LocaleOverride = "ru";
                Assert.Equal("ru", LocalizationBundle.CurrentLocale);

                // An unknown code is passed through; the bundle falls back to values.xml when its file is missing.
                LocalizationBundle.LocaleOverride = "xx";
                Assert.Equal("xx", LocalizationBundle.CurrentLocale);
            }
            finally
            {
                LocalizationBundle.LocaleOverride = null;
            }
        }

        [Fact]
        public void WithoutOverrideTheCultureDecides()
        {
            LocalizationBundle.LocaleOverride = null;
            Assert.Equal(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, LocalizationBundle.CurrentLocale);
        }
    }
}
