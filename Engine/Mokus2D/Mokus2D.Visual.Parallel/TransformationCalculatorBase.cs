namespace Mokus2D.Visual.Parallel;

public abstract class TransformationCalculatorBase(RootNode root)
{
    protected readonly RootNode Root = root;

    public abstract void DoTransformations();
}
