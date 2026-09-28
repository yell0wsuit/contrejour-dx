namespace Mokus2D.Visual.Parallel;

public abstract class TransformationCalculatorBase
{
    protected readonly RootNode Root;

    protected TransformationCalculatorBase(RootNode root)
    {
        Root = root;
    }

    public abstract void DoTransformations();
}
