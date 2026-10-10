using System.Numerics;

using Mokus2D.Effects.Tweening;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class Actions
    {
        public static void ShakeWithDurationPositionOffsetCountScaleDiff(Node node, float time, Vector2 position, float offset, int count, float scaleDiff)
        {
            Sequence sequence = node.Tweener.StartSequence(time / count);
            for (int i = 0; i < count; i++)
            {
                bool flag = i == count - 1;
                _ = sequence.Tween(targetValue: position + (flag ? Vector2.Zero : new Vector2(Maths.Random(0f - offset, offset), Maths.Random(0f - offset, offset))), getSet: NodeValues.Position);
                if (scaleDiff != 0f)
                {
                    float num = 1f;
                    if (!flag)
                    {
                        num = num + (i / (float)count * scaleDiff) + ((i % 2 != 0) ? 0.05f : (-0.05f));
                    }
                    _ = sequence.Tween(NodeValues.Scale, num);
                }
                if (!flag)
                {
                    sequence = sequence.Next(time / count);
                }
            }
        }

        public static void ShakeWithDurationOffsetCount(Node node, float d, float offset, int count)
        {
            ShakeWithDurationPositionOffsetCountScaleDiff(node, d, Vector2.Zero, offset, count, 0.2f);
        }
    }
}
