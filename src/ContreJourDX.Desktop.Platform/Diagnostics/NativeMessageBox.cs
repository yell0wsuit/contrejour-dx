using System;
using System.Runtime.InteropServices;
using System.Text;

using SDL3;

namespace ContreJourDX.Desktop.Platform.Diagnostics
{
    // A blocking platform message box with custom buttons, usable with no SDL window.
    internal static class NativeMessageBox
    {
        internal readonly record struct Button(int Id, string Text, SDL.MessageBoxButtonFlags Flags = 0);

        // Returns the pressed button's id, or fallback when the box could not be shown. The buttons
        // are marshalled by hand: SDL wants an array of its own layout behind a pointer, and the
        // binding's managed struct needs the reflection marshaller to lay one out.
        public static unsafe int Show(SDL.MessageBoxFlags flags, string title, string message, int fallback, params Button[] buttons)
        {
            NativeButton* native = (NativeButton*)NativeMemory.AllocZeroed((nuint)(buttons.Length * sizeof(NativeButton)));
            try
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    native[i] = new NativeButton
                    {
                        Flags = (uint)buttons[i].Flags,
                        ButtonId = buttons[i].Id,
                        Text = Marshal.StringToCoTaskMemUTF8(buttons[i].Text),
                    };
                }
                SDL.MessageBoxData data = new()
                {
                    Flags = flags,
                    Window = 0,
                    Title = title,
                    Message = WrapMessage(message),
                    NumButtons = buttons.Length,
                    Buttons = (nint)native,
                    ColorScheme = 0,
                };
                return SDL.ShowMessageBox(in data, out int pressed) ? pressed : fallback;
            }
            finally
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (native[i].Text != 0)
                    {
                        Marshal.FreeCoTaskMem(native[i].Text);
                    }
                }
                NativeMemory.Free(native);
            }
        }

        // Explicit line breaks at 80 columns, for native boxes that do not wrap.
        internal static string WrapMessage(string message)
        {
            const int columns = 80;
            StringBuilder result = new();
            foreach (string paragraph in message.ReplaceLineEndings("\n").Split('\n'))
            {
                string remaining = paragraph;
                while (remaining.Length > columns)
                {
                    int end = remaining.LastIndexOf(' ', columns, columns + 1);
                    bool atSpace = end > 0;
                    if (!atSpace)
                    {
                        end = columns;
                        if (char.IsHighSurrogate(remaining[end - 1]))
                        {
                            end--;
                        }
                    }
                    _ = result.Append(remaining.AsSpan(0, end)).Append('\n');
                    remaining = remaining[(end + (atSpace ? 1 : 0))..];
                }
                _ = result.Append(remaining).Append('\n');
            }
            return result.ToString(0, result.Length - 1);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeButton
        {
            public uint Flags;
            public int ButtonId;
            public nint Text;
        }
    }
}
