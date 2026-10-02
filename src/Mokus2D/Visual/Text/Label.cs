using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

using Mokus2D.Data;
using Mokus2D.Graphics;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Text
{
    // Text in the current locale's font face (Mokus2DGame.Fonts). Lines are laid out by TextLayout and
    // drawn whole, one renderer text call each, so a label has no child nodes per character.
    public class Label : AnchorNode
    {
        public const char NewLineChar = '\n';

        public const char SkipChar = '\r';

        public const char Nbsp = '\u00a0';

        public const char Space = ' ';

        private static readonly Vector2 DefaultMargins = new(2f);

        private readonly StringBuilder _text = new();

        private readonly List<TextLine> _lines = new(4);

        private readonly bool _dynamicTextureSize = true;

        private readonly IFontFace _face;

        private readonly float _lineHeight;

        private readonly float _emSize;

        private bool _textDirty;

        private float? _lineAnchor;

        public bool DynamicClickArea { get; set; } = true;

        public Vector2 TextSize { get; private set; }

        public float FontSize { get; }

        public override Vector2 Size => !DynamicClickArea ? TextureSize : TextSize;

        public TextAlign Align { get; set; } = TextAlign.Center;

        public override Vector2 Anchor
        {
            get => base.Anchor;
            set
            {
                base.Anchor = value;
                _lineAnchor = null;
            }
        }

        public int TextLength => _text.Length;

        public string TextString
        {
            get => _text.ToString();
            set
            {
                _ = _text.Clear();
                _ = _text.Append(value);
                SetTextDirty();
            }
        }

        public float LineSpacing
        {
            get; set
            {
                if (field != value)
                {
                    field = value;
                    SetTextDirty();
                }
            }
        }

        public char this[int index]
        {
            get => _text[index];
            set
            {
                SetTextDirty();
                _text[index] = value;
            }
        }

        // A label that sizes itself to its text. Like the original, it starts anchored at its top-left
        // corner and left-aligned; ContreJourLabelUtil re-centers the game's labels.
        public Label(float fontSize)
            : this(fontSize, Vector2.Zero)
        {
            _dynamicTextureSize = true;
        }

        // A label laid out inside a fixed area of the given size.
        public Label(float fontSize, Vector2 size)
            : base(null)
        {
            FontRegistry fonts = Mokus2DGame.Fonts;
            _face = fonts.Face;
            FontSize = fontSize;
            _lineHeight = fonts.GetLineHeight(fontSize);
            _emSize = fonts.GetEmSize(_lineHeight);
            ColorRatio = 1f;
            _dynamicTextureSize = false;
            TextureSize = size;
            Anchor = Vector2.Zero;
            Align = TextAlign.Left;
            UpdateChildren = false;
        }

        public void SetText(char value)
        {
            _ = Clear();
            _ = Append(value);
        }

        public void SetText(float value)
        {
            _ = Clear();
            _ = Append(value);
        }

        public void SetText(int value)
        {
            _ = Clear();
            _ = Append(value);
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (Root == null)
            {
                return;
            }
            RefreshText();
            if (_lineAnchor.HasValue)
            {
                if (_lines.Count > 0)
                {
                    AnchorY = _lineAnchor.Value / _lines.Count;
                }
                _lineAnchor = null;
            }
        }

        public void RefreshText()
        {
            if (!_textDirty)
            {
                return;
            }
            Vector2 size = TextLayout.Build(_text.ToString(), _face, _emSize, _lineHeight, LineSpacing, DefaultMargins, _lines);
            if (_dynamicTextureSize)
            {
                TextureSize = size;
            }
            TextSize = size;
            _textDirty = false;
            if (ProcessMouseOver)
            {
                ProcessMouseOver = false;
                ProcessMouseOver = true;
            }
        }

        protected override RectangleFloat CalculateBounds()
        {
            RectangleFloat result = base.CalculateBounds();
            if (DynamicClickArea)
            {
                switch (Align)
                {
                    case TextAlign.Center:
                        result.Offset(((0f - result.Width) / 2f) + (TextureSize.X / 2f), 0f);
                        break;
                    case TextAlign.Right:
                        result.Offset(0f - result.Width + TextureSize.X, 0f);
                        break;
                    case TextAlign.Left:
                    default:
                        break;
                }
            }
            return result;
        }

        public Label Append(char value, int repeatCount)
        {
            SetTextDirty();
            _ = _text.Append(value, repeatCount);
            return this;
        }

        public Label Append(char[] value, int startIndex, int charCount)
        {
            SetTextDirty();
            _ = _text.Append(value, startIndex, charCount);
            return this;
        }

        public Label Append(string value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(string value, int startIndex, int count)
        {
            SetTextDirty();
            _ = _text.Append(value, startIndex, count);
            return this;
        }

        public Label AppendLine()
        {
            SetTextDirty();
            _ = _text.Append('\n');
            return this;
        }

        public Label AppendLine(string value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            _ = AppendLine();
            return this;
        }

        public Label Insert(int index, string value, int count)
        {
            SetTextDirty();
            _ = _text.Insert(index, value, count);
            return this;
        }

        public Label Remove(int startIndex, int length)
        {
            SetTextDirty();
            _ = _text.Remove(startIndex, length);
            return this;
        }

        public Label Append(bool value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(sbyte value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(byte value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(char value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(short value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(int value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(long value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(float value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(double value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(decimal value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(ushort value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(uint value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(ulong value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(object value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Append(char[] value)
        {
            SetTextDirty();
            _ = _text.Append(value);
            return this;
        }

        public Label Insert(int index, string value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, bool value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, sbyte value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, byte value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, short value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, char value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, char[] value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, char[] value, int startIndex, int charCount)
        {
            SetTextDirty();
            _ = _text.Insert(index, value, startIndex, charCount);
            return this;
        }

        public Label Insert(int index, int value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, long value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, float value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, double value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, decimal value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, ushort value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, uint value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, ulong value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label Insert(int index, object value)
        {
            SetTextDirty();
            _ = _text.Insert(index, value);
            return this;
        }

        public Label AppendFormat(string format, object arg0)
        {
            SetTextDirty();
            _ = _text.AppendFormat(CultureInfo.CurrentCulture, format, arg0);
            return this;
        }

        public Label AppendFormat(string format, object arg0, object arg1)
        {
            SetTextDirty();
            _ = _text.AppendFormat(CultureInfo.CurrentCulture, format, arg0, arg1);
            return this;
        }

        public Label AppendFormat(string format, object arg0, object arg1, object arg2)
        {
            SetTextDirty();
            _ = _text.AppendFormat(CultureInfo.CurrentCulture, format, arg0, arg1, arg2);
            return this;
        }

        public Label AppendFormat(string format, params object[] args)
        {
            SetTextDirty();
            _ = _text.AppendFormat(CultureInfo.CurrentCulture, format, args);
            return this;
        }

        public Label AppendFormat(IFormatProvider provider, string format, params object[] args)
        {
            SetTextDirty();
            _ = _text.AppendFormat(provider, format, args);
            return this;
        }

        public Label Replace(string oldValue, string newValue)
        {
            SetTextDirty();
            _ = _text.Replace(oldValue, newValue);
            return this;
        }

        public Label Replace(string oldValue, string newValue, int startIndex, int count)
        {
            SetTextDirty();
            _ = _text.Replace(oldValue, newValue, startIndex, count);
            return this;
        }

        public Label Replace(char oldChar, char newChar)
        {
            SetTextDirty();
            _ = _text.Replace(oldChar, newChar);
            return this;
        }

        public Label Replace(char oldChar, char newChar, int startIndex, int count)
        {
            SetTextDirty();
            _ = _text.Replace(oldChar, newChar, startIndex, count);
            return this;
        }

        public Label Clear()
        {
            SetTextDirty();
            _ = _text.Clear();
            return this;
        }

        private void SetTextDirty()
        {
            _textDirty = true;
        }

        public void SetVerticalAnchorToLine(int lineNumber, float inLineAnchor = 0.5f)
        {
            _lineAnchor = lineNumber + inLineAnchor;
        }

        protected override void BeginDraw(VisualState state)
        {
        }

        // color is the node's straight-alpha color, the same a glyph sprite under this label received.
        protected override void DrawSprite(VisualState state, Color color)
        {
            if (_lines.Count == 0)
            {
                return;
            }
            // Sprites batched before this label must reach the target first.
            Drawer.EndDraw();
            Matrix4x4 screen = state.GetCombinedScreenMatrix(Root.Size);
            float sign = Root.SpritesScaleFactor.Y.Sign();
            // Text rises toward −y of its own space; flipping by sign keeps it upright whichever way the
            // root's Y axis points, as the glyph sprites' quads did.
            Matrix4x4 flip = Matrix4x4.CreateScale(1f, sign, 1f);
            Vector2 start = (-AnchorInPixels * Root.SpritesScaleFactor) + DefaultMargins;
            float ascent = -_face.GetMetrics(_emSize).Ascent;
            for (int i = 0; i < _lines.Count; i++)
            {
                TextLine line = _lines[i];
                if (line.Text.Length == 0)
                {
                    continue;
                }
                float offset = TextLayout.LineOffset(Align, TextureSize.X, line.Width);
                Vector2 origin = TextLayout.LineOrigin(start, offset, i, _lineHeight, LineSpacing, ascent, sign);
                Matrix4x4 transform = flip * Matrix4x4.CreateTranslation(origin.X, origin.Y, 0f) * screen;
                Mokus2DGame.Renderer.DrawText(_face, line.Text, _emSize, transform, color);
            }
        }
    }
}
