using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tweening;
using Mokus2D.Effects.Tweening.Repeating;
using Mokus2D.Fonts;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Text;

public class Cursor : Sprite
{
    private const char CursorSymbol = '|';

    private const float VisibleTime = 0.5f;

    private FontData _fontData;

    public Cursor(FontData data, float scaleFactor)
        : base(data['|'])
    {
        _fontData = data;
        ScaleFactor = scaleFactor;
        IgnoreParentColor = true;
        StartTween();
    }

    public override void ReloadData()
    {
        _fontData = Mokus2DGame.LoadResource<FontData>(_fontData.Id);
        ResetData(_fontData['|']);
    }

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
    }

    public override void UpdateNode(float time)
    {
        base.UpdateNode(time);
    }

    private void StartTween()
    {
        Tweener.Start(RepeatForever.New(Sequence.New(this).Next(0.5f).SetAfter(NodeValues.OpacityFloat, 0f)
            .Next(0.5f)
            .SetAfter(NodeValues.OpacityFloat, 1f)));
    }

    protected override void DrawSprite(VisualState state, Color color)
    {
        base.DrawSprite(state, color);
    }
}
