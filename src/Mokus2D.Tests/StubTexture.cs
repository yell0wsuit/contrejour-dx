using Mokus2D.Graphics;

namespace Mokus2D.Tests
{
    internal sealed class StubTexture : ITexture
    {
        public string Name { get; set; }

        public int Width => 1;

        public int Height => 1;

        public bool IsDisposed => false;

        public void Dispose()
        {
        }
    }
}
