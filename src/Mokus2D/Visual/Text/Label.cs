using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Fonts;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Text.LabelData;

namespace Mokus2D.Visual.Text
{
    public class Label : AnchorNode, IDataReloadable
    {
        public const char Dots = '…';

        public const char NewLineChar = '\n';

        public const char SkipChar = '\r';

        public const char Nbsp = '\u00a0';

        public const char Space = ' ';

        public static readonly bool IgnoreMissingSymbolsDefault;

        private static readonly Vector2 DefaultMargins = new(2f);
        private readonly StringBuilder _text = new();
        private bool _textDirty;

        private bool PositionsDirty;

        private float? _lineAnchor;

        private RectangleFloat? _symbolsBounds;

        private readonly bool _dynamicTextureSize = true;
        public bool DynamicClickArea { get; set; } = true;

        public bool? IgnoreMissingSymbols { get; set; }

        public Vector2 TextSize { get; private set; }

        public float FontSize { get; private set; }

        protected bool ShouldIgnoreMissingSymbols => IgnoreMissingSymbols ?? IgnoreMissingSymbolsDefault;

        public RectangleFloat? SymbolsBounds
        {
            get
            {
                RefreshText();
                RefreshPositions();
                if (!_symbolsBounds.HasValue && Lines.Count != 0 && Lines[0].Glyphs.Count != 0)
                {
                    CalculateSymbolsBounds();
                }
                return _symbolsBounds;
            }
        }

        public List<Glyph> Glyphs { get; } = new(64);

        public List<LabelLine> Lines { get; } = new(64);

        public override Vector2 Size => !DynamicClickArea ? TextureSize : TextSize;

        public TextAlign Align
        {
            get; set
            {
                if (field != value)
                {
                    field = value;
                    PositionsDirty = true;
                }
            }
        } = TextAlign.Center;

        public override Vector2 Anchor
        {
            get => base.Anchor;
            set
            {
                if (base.Anchor != value)
                {
                    base.Anchor = value;
                    PositionsDirty = true;
                }
                _lineAnchor = null;
            }
        }

        public int TextLength => _text.Length;

        public FontData Font
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
                    PositionsDirty = true;
                }
            }
        }

        public bool LimitWidth
        {
            get => MaxWidth.HasValue;
            set => MaxWidth = value ? new float?(TextureSize.X) : null;
        }

        public float? MaxWidth
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

        public Label(string fontName, float fontSize)
            : this(fontName, fontSize, Vector2.Zero)
        {
            _dynamicTextureSize = true;
        }

        public Label(string fontName, float fontSize, Vector2 size)
            : this(Mokus2DGame.FontsManager.GetFontData(fontName, fontSize))
        {
            _dynamicTextureSize = false;
            FontSize = fontSize;
            TextureSize = size;
            RecalculateScaleFactor();
            Anchor = Vector2.Zero;
            Align = TextAlign.Left;
            UpdateChildren = false;
        }

        public Label(string id)
            : this(Mokus2DGame.LoadResource<FontData>(id))
        {
        }

        public Label(FontData font)
            : base(null)
        {
            Font = font;
            ScaleFactor = font.ScaleFactor;
            ColorRatio = 1f;
        }

        public void ReloadData()
        {
            Font = Mokus2DGame.FontsManager.GetFontData(Font.FontName, FontSize);
            RecalculateScaleFactor();
            foreach (Glyph glyph in Glyphs)
            {
                glyph.ReloadData(Font);
            }
            SetTextDirty();
        }

        private void RecalculateScaleFactor()
        {
            ScaleFactor = FontSize / Font.FontSize * Font.ScaleFactor;
        }

        public Mokus2D.Util.Data.Point Get2DSymbolPosition(int position)
        {
            Mokus2D.Util.Data.Point result = default;
            for (int i = 0; Lines[i].Glyphs.Count + 1 < position; i++)
            {
                position -= Lines[i].Glyphs.Count + 1;
            }
            result.X = position;
            return result;
        }

        public int GetSymbolPosition(Mokus2D.Util.Data.Point position)
        {
            int num = 0;
            for (int i = 0; i < position.Y; i++)
            {
                num += Lines[i].Glyphs.Count + 1;
            }
            return num + position.X;
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

        public void RemoveFirstLine()
        {
            int newLine = IndexOfNewLine();
            int length = newLine < 0 ? _text.Length : newLine + 1;
            _ = _text.Remove(0, length);
            SetTextDirty();
        }

        // StringBuilder has no IndexOf, and searching _text directly avoids copying it into a string.
        private int IndexOfNewLine()
        {
            for (int i = 0; i < _text.Length; i++)
            {
                if (_text[i] == '\n')
                {
                    return i;
                }
            }
            return -1;
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
                if (Lines.Count > 0)
                {
                    AnchorY = _lineAnchor.Value / Lines.Count;
                }
                _lineAnchor = null;
            }
            RefreshPositions();
        }

        private void RefreshPositions()
        {
            if (PositionsDirty)
            {
                DoRefreshPositions();
                PositionsDirty = false;
            }
        }

        public void RefreshText()
        {
            if (!_textDirty)
            {
                return;
            }
            try
            {
                DoRefreshText();
                _textDirty = false;
                if (ProcessMouseOver)
                {
                    ProcessMouseOver = false;
                    ProcessMouseOver = true;
                }
            }
            catch (IndexOutOfRangeException)
            {
                _textDirty = true;
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

        public Vector2 GetGlyphLeftTop(Mokus2D.Util.Data.Point positionInText)
        {
            Vector2 result = StartGlyphsPosition();
            result.Y += ((Font.RealHeight * ScaleFactor) + LineSpacing) * Root.SpritesScaleFactor.Y.Sign() * positionInText.Y;
            LabelLine labelLine = Lines[positionInText.Y];
            result.X += GetLineHorizontalOffset(labelLine);
            for (int i = 0; i < positionInText.X; i++)
            {
                Glyph glyph = labelLine.Glyphs[i];
                result.X += glyph.Width;
            }
            return result;
        }

        public void RefreshSymbolsTransform()
        {
            DoRefreshPositions();
            foreach (Glyph glyph in Glyphs)
            {
                glyph.Scale = 1f;
                glyph.OpacityFloat = 1f;
            }
        }

        private void DoRefreshPositions()
        {
            Vector2 vector = StartGlyphsPosition();
            float num = Root.SpritesScaleFactor.Y.Sign();
            float num2 = Font.RealHeight / 2f * ScaleFactor;
            foreach (LabelLine line in Lines)
            {
                float lineHorizontalOffset = GetLineHorizontalOffset(line);
                for (int i = 0; i < line.Glyphs.Count; i++)
                {
                    Glyph glyph = line.Glyphs[i];
                    glyph.Position = vector + new Vector2((glyph.Width / 2f) + lineHorizontalOffset, num2 * num);
                    vector.X += glyph.Width;
                }
                vector.Y += ((Font.RealHeight * ScaleFactor) + LineSpacing) * num;
                vector.X = StartGlyphsPosition().X;
            }
        }

        private void CalculateSymbolsBounds()
        {
            foreach (LabelLine line in Lines)
            {
                foreach (Glyph glyph in line.Glyphs)
                {
                    RectangleFloat bounds = glyph.Bounds;
                    if (bounds.Width != 0f && bounds.Height != 0f)
                    {
                        bounds.Offset(glyph.Position);
                        _symbolsBounds = !_symbolsBounds.HasValue ? bounds : RectangleFloat.Union(_symbolsBounds.Value, bounds);
                    }
                }
            }
        }

        protected virtual Vector2 StartGlyphsPosition()
        {
            return (-AnchorInPixels * Root.SpritesScaleFactor) + DefaultMargins;
        }

        private float GetLineHorizontalOffset(LabelLine line)
        {
            return Align switch
            {
                TextAlign.Left => 0f,
                TextAlign.Right => TextureSize.X - line.Width,
                TextAlign.Center => (TextureSize.X - line.Width) / 2f,
                _ => throw new InvalidOperationException(),
            };
        }

        private LabelLine GetCleanLine(int index)
        {
            if (Lines.Count <= index)
            {
                Lines.Add(LabelLine.New());
            }
            LabelLine labelLine = Lines[index];
            labelLine.Glyphs.Clear();
            return labelLine;
        }

        protected virtual void DoRefreshText()
        {
            Vector2 vector = new(0f, (DefaultMargins.Y * 2f) + (Font.RealHeight * ScaleFactor) + LineSpacing);
            float num = DefaultMargins.X * 2f;
            int num2 = 0;
            LabelLine cleanLine = GetCleanLine(num2);
            int num3 = 0;
            bool flag = false;
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];
                if (c == '\r')
                {
                    continue;
                }
                if (c == '\u00a0')
                {
                    c = ' ';
                }
                if (IsNewLine(c))
                {
                    if (i != _text.Length - 1)
                    {
                        vector.Y += (Font.RealHeight * ScaleFactor) + LineSpacing;
                    }
                    vector.X = Math.Max(num, vector.X);
                    cleanLine.Width = num;
                    cleanLine = GetCleanLine(++num2);
                    num = DefaultMargins.X * 2f;
                    flag = false;
                    continue;
                }
                CharData charData = Font[c];
                if (charData != null && !flag && (!MaxWidth.HasValue || num < MaxWidth))
                {
                    Glyph glyph = RefreshGlyph(num3, c, charData);
                    if (MaxWidth.HasValue && num + glyph.Width >= MaxWidth)
                    {
                        _ = glyph.Initialize(Font['…'], ScaleFactor, '…');
                        flag = true;
                    }
                    glyph.Visible = true;
                    num3++;
                    num += glyph.Width;
                    cleanLine.Glyphs.Add(glyph);
                }
                else if (!ShouldIgnoreMissingSymbols)
                {
                    throw new KeyNotFoundException("Symbol not found: " + c);
                }
            }
            vector.X = Math.Max(num, vector.X);
            cleanLine.Width = num;
            if (_dynamicTextureSize)
            {
                TextureSize = vector;
            }
            TextSize = vector;
            FreeUnusedObjects(num2, num3);
            PositionsDirty = true;
        }

        private void FreeUnusedObjects(int lineIndex, int glyphCount)
        {
            int num = Lines.Count - (lineIndex + 1);
            if (num > 0)
            {
                for (int i = lineIndex + 1; i < Lines.Count; i++)
                {
                    LabelLine.Free(Lines[i]);
                }
                Lines.RemoveRange(lineIndex + 1, num);
            }
            int num2 = Glyphs.Count - glyphCount;
            if (num2 > 0)
            {
                for (int num3 = Glyphs.Count - 1; num3 >= glyphCount; num3--)
                {
                    Glyph glyph = Glyphs[num3];
                    RemoveChild(glyph);
                    Glyph.Free(glyph);
                }
                Glyphs.RemoveRange(glyphCount, num2);
            }
        }

        private static bool IsNewLine(char symbol)
        {
            return '\n' == symbol;
        }

        protected virtual Glyph RefreshGlyph(int index, char symbol, CharData data)
        {
            Glyph glyph;
            if (Glyphs.Count <= index)
            {
                glyph = Glyph.New(data, ScaleFactor, symbol);
                Glyphs.Add(glyph);
                AddChild(glyph);
            }
            else
            {
                glyph = Glyphs[index];
                _ = glyph.Initialize(data, ScaleFactor, symbol);
            }
            glyph.Scale = 1f;
            glyph.OpacityFloat = 1f;
            glyph.IgnoreParentColor = false;
            return glyph;
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
            _symbolsBounds = null;
            _textDirty = true;
        }

        public void SetSymbolsColor(Color color, int index, int count)
        {
            SetSymbolsColor(color, 1f, index, count);
        }

        public void SetSymbolsColor(Color color, float colorRatio, int index, int count)
        {
            RefreshText();
            for (int i = index; i < index + count; i++)
            {
                Glyph glyph = Glyphs[i];
                glyph.Color = color;
                glyph.ColorRatio = colorRatio;
                glyph.IgnoreParentColor = true;
            }
        }

        public void SetVerticalAnchorToLine(int lineNumber, float inLineAnchor = 0.5f)
        {
            _lineAnchor = lineNumber + inLineAnchor;
        }

        protected override void BeginDraw(VisualState state)
        {
        }

        protected override void DrawSprite(VisualState state, Color color)
        {
        }

        public override void Draw(VisualState state)
        {
            if (Font.Texture.IsDisposed)
            {
                ReloadData();
            }
            base.Draw(state);
        }
    }
}
