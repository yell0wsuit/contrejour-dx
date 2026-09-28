using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Parallel;

public class OneThreadTransformCalculator : TransformationCalculatorBase
{
    public OneThreadTransformCalculator(RootNode root)
        : base(root)
    {
    }

    public override void DoTransformations()
    {
        if (!TransformationUtil.ShouldRefreshNode(Root))
        {
            return;
        }
        Root.RefreshVisualState(Root.RootState);
        foreach (Node child in Root.Children)
        {
            RefreshTransformations(child);
        }
    }

    private static void RefreshTransformations(Node node)
    {
        if (!TransformationUtil.ShouldRefreshNode(node))
        {
            return;
        }
        node.RefreshVisualState(node.Parent.CompositeState);
        if (!node.UpdateChildrenTransformations)
        {
            return;
        }
        foreach (Node child in node.Children)
        {
            RefreshTransformations(child);
        }
    }
}
