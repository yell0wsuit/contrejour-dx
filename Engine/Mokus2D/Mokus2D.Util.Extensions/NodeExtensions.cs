using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Animation;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Util;

namespace Mokus2D.Util.Extensions;

public static class NodeExtensions
{
	public static void ApplyChildTransformations(this AnimationNode target, AnimationNode source, bool recursive = false)
	{
		AnimationUtil.ApplyChildTransformations(source, target, recursive);
	}

	public static void HideAndIgnoreAnimation(this Node node)
	{
		node.Visible = false;
		node.IgnoredAnimations.Visible = true;
	}

	public static void SetChildrenBlend(this Node node, BlendState blend, bool recursive = false)
	{
		foreach (Node child in node.Children)
		{
			if (child is IBlendable blendable)
			{
				blendable.Blend = blend;
			}
			if (recursive)
			{
				child.SetChildrenBlend(blend, recursive: true);
			}
		}
	}

	public static void ReloadOffspringTextures(this Node parent)
	{
		parent.ApplyToAllOffsprings(ReloadTexture);
	}

	private static void ReloadTexture(Node node)
	{
		if (node is IDataReloadable dataReloadable)
		{
			dataReloadable.ReloadData();
		}
	}
}
