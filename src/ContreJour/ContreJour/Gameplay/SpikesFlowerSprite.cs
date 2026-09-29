using System.Collections.Generic;

using ContreJour.Primitives;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay;

public class SpikesFlowerSprite : LongNeckSprite
{
    private readonly SpikesFlowerBodyClip spikes;

    private Pair<Vector2> basePoints;

    private Pair<Vector2> centerPoints;

    private readonly float childScale;

    public SpikesFlowerSprite(SpikesFlowerBodyClip bodyClip, float scale)
    {
        if (bodyClip.Game.WhiteSide)
        {
            NeckColor = 10066329.ToRGBColor();
        }
        else if (bodyClip.Game.BonusChapter)
        {
            NeckColor = ContreJourConstants.GreenSpikesFlower;
        }
        spikes = bodyClip;
        basePoints = new Pair<Vector2>(new Vector2(6f, -32f) * scale, new Vector2(-6f, -32f) * scale);
        centerPoints = new Pair<Vector2>(new Vector2(-3f, 0f) * scale, new Vector2(3f, 0f) * scale);
        childScale = scale;
    }

    public override void GetPairs(List<Pair<Vector2>> target)
    {
        Vector2 position = spikes.Eye.Position;
        target.Add(new Pair<Vector2>(position + new Vector2(5f * childScale, 0f), position - new Vector2(5f * childScale, 0f)));
        target.Add(centerPoints);
        target.Add(basePoints);
    }
}
