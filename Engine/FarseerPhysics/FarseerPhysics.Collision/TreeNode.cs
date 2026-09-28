namespace FarseerPhysics.Collision;

internal class TreeNode<T>
{
	internal AABB AABB;

	internal int Child1;

	internal int Child2;

	internal int Height;

	internal int ParentOrNext;

	internal T UserData;

	internal bool IsLeaf()
	{
		return Child1 == -1;
	}
}
