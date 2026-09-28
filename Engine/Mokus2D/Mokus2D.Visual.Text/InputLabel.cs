using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mokus2D.Fonts;
using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Focus;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Text;

public class InputLabel : Label, IFocus
{
    public string AllowedSymbols;

    private bool _hasFocus;

    private bool _listenersAdded;

    private Mokus2D.Util.Data.Point _textPosition = default(Mokus2D.Util.Data.Point);

    private Cursor _cursor;

    private bool _cursorPositionDirty = true;

    private bool _positionToEnd = true;

    private bool _enabled = true;

    private readonly Dictionary<Keys, Action<Keys>> _keyHandlers = new Dictionary<Keys, Action<Keys>>();

    private int? _maxSymbols;

    public int? MaxSymbols
    {
        get
        {
            return _maxSymbols;
        }
        set
        {
            _maxSymbols = value;
        }
    }

    public bool Enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                if (!value)
                {
                    HasFocus = false;
                }
            }
        }
    }

    public Color CursorColor
    {
        get
        {
            return _cursor.Color;
        }
        set
        {
            _cursor.Color = value;
            _cursor.ColorRatio = 1f;
        }
    }

    public bool HasFocus
    {
        get
        {
            return _hasFocus;
        }
        set
        {
            if (_hasFocus != value)
            {
                _hasFocus = value;
                _cursor.Visible = value;
                RefreshKeyboardListeners();
                if (_hasFocus)
                {
                    this.FocusInEvent.Dispatch(this);
                }
                else
                {
                    this.FocusOutEvent.Dispatch(this);
                }
            }
        }
    }

    public Mokus2D.Util.Data.Point TextPosition
    {
        get
        {
            return _textPosition;
        }
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

    public bool CursorAtEnd
    {
        get
        {
            if (TextPosition.Y < base.Lines.Count)
            {
                if (TextPosition.Y == base.Lines.Count - 1)
                {
                    return TextPosition.X >= base.Lines.Last().Glyphs.Count;
                }
                return false;
            }
            return true;
        }
    }

    public bool CursorAtStart
    {
        get
        {
            if (TextPosition.Y >= 0)
            {
                if (TextPosition.Y == 0)
                {
                    return TextPosition.X <= 0;
                }
                return false;
            }
            return true;
        }
    }

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
                TextPosition = Get2DSymbolPosition(base.TextLength);
            }
            _positionToEnd = false;
            _cursorPositionDirty = false;
            _cursor.Position = GetGlyphLeftTop(_textPosition) + _cursor.ScaledSize / 2f;
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
        textPosition.Y = Math.Min(textPosition.Y, base.Lines.Count - 1);
        textPosition.X = Math.Min(textPosition.X, base.Lines[textPosition.Y].Glyphs.Count);
        TextPosition = textPosition;
    }

    private Cursor CreatCursor()
    {
        Cursor cursor = new Cursor(base.Font, ScaleFactor);
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
        bool flag = base.OnDisplayList && HasFocus;
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

    private void TryAddSymbol(Keys key)
    {
        if (MaxSymbols.HasValue && base.TextLength >= MaxSymbols.Value)
        {
            return;
        }
        char? c = TextUtil.KeyToChar(key, Mokus2DGame.Keyboard.IsCapital);
        if (((int?)c).HasValue && (AllowedSymbols == null || Enumerable.Contains(AllowedSymbols, c.Value)))
        {
            CharData charData = base.Font[c.Value];
            if (charData != null && (!base.MaxWidth.HasValue || !(base.TextSize.X + charData.Width > base.MaxWidth)))
            {
                RefreshText();
                Insert(GetSymbolPosition(TextPosition), c);
                TextPosition += new Mokus2D.Util.Data.Point(1, 0);
                this.TextChangeEvent.Dispatch();
            }
        }
    }

    private void OnBackspace(Keys keys)
    {
        if (!CursorAtStart)
        {
            int symbolPosition = GetSymbolPosition(TextPosition);
            symbolPosition--;
            SafeRemove(symbolPosition, 1);
            TextPosition = Get2DSymbolPosition(symbolPosition);
            this.TextChangeEvent.Dispatch();
        }
    }

    private void OnDelete(Keys keys)
    {
        if (!CursorAtEnd)
        {
            SafeRemove(GetSymbolPosition(TextPosition), 1);
            this.TextChangeEvent.Dispatch();
        }
    }

    private void SafeRemove(int startIndex, int length)
    {
        if (startIndex >= 0 && startIndex <= base.TextLength && length >= 0 && startIndex + length <= base.TextLength)
        {
            Remove(startIndex, length);
        }
    }

    private void MoveHorizontal(int direction)
    {
        int symbolPosition = GetSymbolPosition(TextPosition);
        symbolPosition += direction;
        symbolPosition = symbolPosition.Clamp(0, base.TextLength);
        TextPosition = Get2DSymbolPosition(symbolPosition);
    }

    private void OnKeyPressed(Keys keys)
    {
        Action<Keys> action = _keyHandlers.TryGetValue(keys);
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
        _keyHandlers.Add(Keys.Enter, OnEnter);
        _keyHandlers.Add(Keys.Escape, OnEscape);
        _keyHandlers.Add(Keys.Back, OnBackspace);
        _keyHandlers.Add(Keys.Delete, OnDelete);
        _keyHandlers.Add(Keys.Left, delegate
        {
            MoveHorizontal(-1);
        });
        _keyHandlers.Add(Keys.Right, delegate
        {
            MoveHorizontal(1);
        });
    }

    private void OnEscape(Keys obj)
    {
        if (HasFocus)
        {
            HasFocus = false;
        }
    }

    private void OnEnter(Keys obj)
    {
    }
}
