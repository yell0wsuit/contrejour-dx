using System;
using System.Collections.Generic;

using SDL3;

namespace ContreJour.Desktop.Platform
{
    // Owns the open gamepad handles (from cuttherope-dx's SdlGamepadService). SDL only delivers
    // button events for opened devices, so handling the events is not enough on its own.
    public sealed class SdlGamepads : IDisposable
    {
        private readonly Func<uint[]> _enumerate;

        private readonly Func<uint, nint> _open;

        private readonly Action<nint> _close;

        private readonly Dictionary<uint, nint> _handles = [];

        private bool _disposed;

        // enumerate: ids of the attached devices; open: returns 0 when a device cannot be opened.
        public SdlGamepads(Func<uint[]> enumerate, Func<uint, nint> open, Action<nint> close)
        {
            ArgumentNullException.ThrowIfNull(enumerate);
            ArgumentNullException.ThrowIfNull(open);
            ArgumentNullException.ThrowIfNull(close);
            _enumerate = enumerate;
            _open = open;
            _close = close;
        }

        public int Count => _handles.Count;

        // Opens the devices attached before the host started listening for events.
        public void OpenConnected()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            foreach (uint id in _enumerate() ?? [])
            {
                Open(id);
            }
        }

        public void HandleEvent(in SDL.Event e)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            SDL.EventType type = (SDL.EventType)e.Type;
            if (type == SDL.EventType.GamepadAdded)
            {
                Open(e.GDevice.Which);
            }
            else if (type == SDL.EventType.GamepadRemoved && _handles.Remove(e.GDevice.Which, out nint removed))
            {
                _close(removed);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            foreach (nint handle in _handles.Values)
            {
                _close(handle);
            }
            _handles.Clear();
        }

        private void Open(uint id)
        {
            if (_handles.ContainsKey(id))
            {
                return;
            }
            // A device that fails to open is not owned, so a later add event may retry it.
            nint handle = _open(id);
            if (handle != 0)
            {
                _handles.Add(id, handle);
            }
        }
    }
}
