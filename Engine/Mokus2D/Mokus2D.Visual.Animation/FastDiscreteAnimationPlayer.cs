using System.Collections.Generic;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Animation;

internal class FastDiscreteAnimationPlayer : IAnimationNodePlayer
{
	public void ApplyFrameData(AnimationNode node, float frame)
	{
		List<AnimationFrameData> list = node.AnimationData[(int)frame];
		for (int i = 0; i < list.Count; i++)
		{
			Node child = node.Children[i];
			AnimationUtil.ApplyChildFrameData(node, child, list[i]);
		}
	}
}
