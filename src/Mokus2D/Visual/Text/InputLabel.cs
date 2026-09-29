using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Mokus2D.Fonts;
using Mokus2D.Graphics;
using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Focus;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Text
{
    public class InputLabel : Label, IFocus
    {
        public string AllowedSymbols { get; set; }
        private bool _listenersAdded;

        private Mokus2D.Util.Data.Point _textPosition;

        private Cursor _cursor;

        private bool _cursorPositionDirty = true;

        private bool _positionToEnd = true;
        private readonly Dictionary<Key, Action<Key>> _keyHandlers = [];

        public int? MaxSymbols { get; set; }

        public bool Enabled
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    if (!value)
                    {
                        HasFocus = false;
                    }
                }
            }
        } = true;

        public Color CursorColor
        {
            get => _cursor.Color;
            set
            {
                _cursor.Color = value;
                _cursor.ColorRatio = 1f;
            }
        }

        public bool HasFocus
        {
            get; set
            {
                if (field != value)
                {
                    field = value;
                    _cursor.Visible = value;
                    RefreshKeyboardListeners();
                    if (field)
                    {
                        FocusInEvent.Dispatch(this);
                    }
                    else
                    {
                        FocusOutEvent.Dispatch(this);
                    }
                }
            }
        }

        public Mokus2D.Util.Data.Point TextPosition
        {
            get => _textPosition;
            set
            {
                if (_textPosition != value)
                {
                    _textPosition = value;
                    _cursorPositionDirty = true;
                    _positionToEnd = false;
                }
            }
        }

        public bool CursorAtEnd => TextPosition.Y >= Lines.Count || (TextPosition.Y == Lines.Count - 1 && TextPosition.X >= Lines[^1].Glyphs.Count);

        public bool CursorAtStart => TextPosition.Y < 0 || (TextPosition.Y == 0 && TextPosition.X <= 0);

        public event Action<IFocus> FocusInEvent;

        public event Action<IFocus> FocusOutEvent;

        public event Action TextChangeEvent;

        public InputLabel(string fontName, float fontSize, Vector2 size)
            : base(fontName, fontSize, size)
        {
            Initialize();
        }

        public InputLabel(string id)
            : base(id)
        {
            Initialize();
        }

        public InputLabel(FontData font = null)
            : base(font)
        {
            Initialize();
        }

        private void Initialize()
        {
            _cursor = CreatCursor();
            DynamicClickArea = false;
            AddKeyHandlers();
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (_cursorPositionDirty)
            {
                if (_positionToEnd)
                {
                    TextPosition = Get2DSymbolPosition(TextLength);
                }
                _positionToEnd = false;
                _cursorPositionDirty = false;
                _cursor.Position = GetGlyphLeftTop(_textPosition) + (_cursor.ScaledSize / 2f);
            }
        }

        public override void UpdateNode(float time)
        {
            base.UpdateNode(time);
            _cursor.UpdateNode(time);
        }

        public override bool TouchBegin(Touch touch)
        {
            if (Enabled)
            {
                HasFocus = true;
            }
            return base.TouchBegin(touch);
        }

        protected override void DoRefreshText()
        {
            base.DoRefreshText();
            RefreshTextPosition();
        }

        private void RefreshTextPosition()
        {
            Mokus2D.Util.Data.Point textPosition = TextPosition;
            textPosition.Y = Math.Min(textPosition.Y, Lines.Count - 1);
            textPosition.X = Math.Min(textPosition.X, Lines[textPosition.Y].Glyphs.Count);
            TextPosition = textPosition;
        }

        private Cursor CreatCursor()
        {
            Cursor cursor = new(Font, ScaleFactor);
            AddChild(cursor);
            cursor.Visible = false;
            return cursor;
        }

        protected override void OnAddedToStage()
        {
            base.OnAddedToStage();
            RefreshKeyboardListeners();
            FocusManager.AddItem(this);
        }

        protected override void OnRemovedFromStage()
        {
            base.OnRemovedFromStage();
            RefreshKeyboardListeners();
            HasFocus = false;
            FocusManager.RemoveItem(this);
        }

        private void RefreshKeyboardListeners()
        {
            bool flag = OnDisplayList && HasFocus;
            if (flag && !_listenersAdded)
            {
                _listenersAdded = true;
                Mokus2DGame.Keyboard.KeyPressedEvent += OnKeyPressed;
            }
            else if (!flag && _listenersAdded)
            {
                _listenersAdded = false;
                Mokus2DGame.Keyboard.KeyPressedEvent -= OnKeyPressed;
            }
        }

        private void TryAddSymbol(Key key)
        {
            if (MaxSymbols.HasValue && TextLength >= MaxSymbols.Value)
            {
                return;
            }
            char? c = TextUtil.KeyToChar(key, Mokus2DGame.Keyboard.IsCapital);
            if (((int?)c).HasValue && (AllowedSymbols == null || Enumerable.Contains(AllowedSymbols, c.Value)))
            {
                CharData charData = Font[c.Value];
                if (charData != null && (!MaxWidth.HasValue || !(TextSize.X + charData.Width > MaxWidth)))
                {
                    RefreshText();
                    _ = Insert(GetSymbolPosition(TextPosition), c);
                    TextPosition += new Mokus2D.Util.Data.Point(1, 0);
                    TextChangeEvent.Dispatch();
                }
            }
        }

        private void OnBackspace(Key keys)
        {
            if (!CursorAtStart)
            {
                int symbolPosition = GetSymbolPosition(TextPosition);
                symbolPosition--;
                SafeRemove(symbolPosition, 1);
                TextPosition = Get2DSymbolPosition(symbolPosition);
                TextChangeEvent.Dispatch();
            }
        }

        private void OnDelete(Key keys)
        {
            if (!CursorAtEnd)
            {
                SafeRemove(GetSymbolPosition(TextPosition), 1);
                TextChangeEvent.Dispatch();
            }
        }

        private void SafeRemove(int startIndex, int length)
        {
            if (startIndex >= 0 && startIndex <= TextLength && length >= 0 && startIndex + length <= TextLength)
            {
                _ = Remove(startIndex, length);
            }
        }

        private void MoveHorizontal(int direction)
        {
            int symbolPosition = GetSymbolPosition(TextPosition);
            symbolPosition += direction;
            symbolPosition = symbolPosition.Clamp(0, TextLength);
            TextPosition = Get2DSymbolPosition(symbolPosition);
        }

        private void OnKeyPressed(Key keys)
        {
            Action<Key> action = _keyHandlers.GetValueOrDefault(keys);
            if (action != null)
            {
                action(keys);
            }
            else
            {
                TryAddSymbol(keys);
            }
        }

        public void PositionToEnd()
        {
            _positionToEnd = true;
            _cursorPositionDirty = true;
        }

        private void AddKeyHandlers()
        {
            _keyHandlers.Add(Key.Enter, OnEnter);
            _keyHandlers.Add(Key.Escape, OnEscape);
            _keyHandlers.Add(Key.Back, OnBackspace);
            _keyHandlers.Add(Key.Delete, OnDelete);
            _keyHandlers.Add(Key.Left, delegate
            {
                MoveHorizontal(-1);
            });
            _keyHandlers.Add(Key.Right, delegate
            {
                MoveHorizontal(1);
            });
        }

        private void OnEscape(Key obj)
        {
            if (HasFocus)
            {
                HasFocus = false;
            }
        }

        private void OnEnter(Key obj)
        {
        }
    }
}
