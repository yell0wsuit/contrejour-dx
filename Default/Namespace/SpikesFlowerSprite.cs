using System.Collections.Generic;
using ContreJour.Primitives;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class SpikesFlowerSprite : LongNeckSprite
{
    protected SpikesFlowerBodyClip spikes;

    protected Pair<Vector2> basePoints;

    protected Pair<Vector2> centerPoints;

    protected float childScale;

    public SpikesFlowerSprite(SpikesFlowerBodyClip bodyClip, float _scale)
    {
        if (bodyClip.Game.WhiteSide)
        {
            base.NeckColor = 10066329.ToRGBColor();
        }
        else if (bodyClip.Game.BonusChapter)
        {
            base.NeckColor = ContreJourConstants.GreenSpikesFlower;
        }
        spikes = bodyClip;
        basePoints = new Pair<Vector2>(new Vector2(6f, -32f) * _scale, new Vector2(-6f, -32f) * _scale);
        centerPoints = new Pair<Vector2>(new Vector2(-3f, 0f) * _scale, new Vector2(3f, 0f) * _scale);
        childScale = _scale;
    }

    public override void GetPairs(List<Pair<Vector2>> result)
    {
        Vector2 position = spikes.Eye.Position;
        result.Add(new Pair<Vector2>(position + new Vector2(5f * childScale, 0f), position - new Vector2(5f * childScale, 0f)));
        result.Add(centerPoints);
        result.Add(basePoints);
    }
}
