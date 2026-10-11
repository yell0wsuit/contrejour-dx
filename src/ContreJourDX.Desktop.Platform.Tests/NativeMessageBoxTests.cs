using System;

using ContreJourDX.Desktop.Platform.Diagnostics;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    public class NativeMessageBoxTests
    {
        [Fact]
        public void LongLinesAreWrappedAtWordsWithinEightyColumns()
        {
            string message = string.Join(' ', new string('a', 50), new string('b', 50));

            string wrapped = NativeMessageBox.WrapMessage(message);

            Assert.Equal(new string('a', 50) + "\n" + new string('b', 50), wrapped);
        }

        [Fact]
        public void ParagraphsAreKept()
        {
            Assert.Equal("one\n\ntwo", NativeMessageBox.WrapMessage("one" + Environment.NewLine + Environment.NewLine + "two"));
        }
    }
}
