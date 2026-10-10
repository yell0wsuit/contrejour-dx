using System.Collections.Generic;
using System.Linq;

using SDL3;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    public class SdlGamepadsTests
    {
        private readonly List<nint> _closed = [];

        private SdlGamepads Create(params uint[] connected)
        {
            // Handle = id + 1000, and id 13 always fails to open.
            return new SdlGamepads(() => connected, id => id == 13 ? 0 : (nint)(id + 1000), _closed.Add);
        }

        private static SDL.Event Device(SDL.EventType type, uint id)
        {
            SDL.Event e = default;
            e.GDevice.Type = type;
            e.GDevice.Which = id;
            return e;
        }

        [Fact]
        public void OpenConnectedOpensEachAttachedPad()
        {
            using SdlGamepads pads = Create(7, 9);

            pads.OpenConnected();

            Assert.Equal(2, pads.Count);
        }

        [Fact]
        public void AddedAndRemovedOpenAndClose()
        {
            using SdlGamepads pads = Create();

            pads.HandleEvent(Device(SDL.EventType.GamepadAdded, 7));
            pads.HandleEvent(Device(SDL.EventType.GamepadAdded, 7));
            pads.HandleEvent(Device(SDL.EventType.GamepadRemoved, 7));

            Assert.Equal(0, pads.Count);
            Assert.Equal(new nint[] { 1007 }, _closed);
        }

        [Fact]
        public void FailedOpenIsNotOwned()
        {
            using SdlGamepads pads = Create();

            pads.HandleEvent(Device(SDL.EventType.GamepadAdded, 13));

            Assert.Equal(0, pads.Count);
        }

        [Fact]
        public void DisposeClosesEveryPad()
        {
            SdlGamepads pads = Create(7, 9);
            pads.OpenConnected();

            pads.Dispose();

            Assert.Equal([1007, 1009], _closed.Order());
        }
    }
}
