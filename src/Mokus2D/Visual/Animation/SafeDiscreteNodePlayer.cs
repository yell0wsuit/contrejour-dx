using System.Collections.Generic;

using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Animation
{
    public class SafeDiscreteNodePlayer : IAnimationNodePlayer
    {
        public void ApplyFrameData(AnimationNode node, float frame)
        {
            List<AnimationFrameData> list = node.AnimationData[(int)frame];
            foreach (AnimationFrameData item in list)
            {
                Node child = node.GetChild(item.Id, throwOnNotFound: false);
                if (child != null)
                {
                    AnimationUtil.ApplyChildFrameData(node, child, item);
                }
            }
        }
    }
}
