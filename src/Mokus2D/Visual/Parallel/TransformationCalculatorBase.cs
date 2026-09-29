namespace Mokus2D.Visual.Parallel
{
    public abstract class TransformationCalculatorBase(RootNode root)
    {
        protected RootNode Root { get; } = root;

        public abstract void DoTransformations();
    }
}
