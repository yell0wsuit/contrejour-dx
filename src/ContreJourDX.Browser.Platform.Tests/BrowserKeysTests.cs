using Mokus2D.Input;

using Xunit;

namespace ContreJourDX.Browser.Platform.Tests
{
    public class BrowserKeysTests
    {
        [Theory]
        [InlineData(2, Key.Escape)]
        [InlineData(3, Key.F5)]
        [InlineData(4, Key.Back)]
        [InlineData(5, Key.Enter)]
        [InlineData(6, Key.CapsLock)]
        [InlineData(7, Key.Left)]
        [InlineData(8, Key.Up)]
        [InlineData(9, Key.Right)]
        [InlineData(10, Key.Down)]
        [InlineData(11, Key.Delete)]
        [InlineData(12, Key.A)]
        [InlineData(13, Key.D)]
        [InlineData(14, Key.S)]
        [InlineData(15, Key.W)]
        [InlineData(16, Key.LeftShift)]
        [InlineData(17, Key.RightShift)]
        public void IdsMapToTheDesktopKeySet(int id, Key key)
        {
            Assert.Equal(key, BrowserKeys.Map(id));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(18)]
        [InlineData(-1)]
        public void UnknownIdsMapToNone(int id)
        {
            Assert.Equal(Key.None, BrowserKeys.Map(id));
        }
    }
}
