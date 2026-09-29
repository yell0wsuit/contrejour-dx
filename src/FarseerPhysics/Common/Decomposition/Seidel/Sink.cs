namespace FarseerPhysics.Common.Decomposition.Seidel;

internal sealed class Sink : Node
{
    public Trapezoid Trapezoid;

    private Sink(Trapezoid trapezoid)
        : base(null, null)
    {
        Trapezoid = trapezoid;
        trapezoid.Sink = this;
    }

    public static Sink Isink(Trapezoid trapezoid)
    {
        return trapezoid.Sink ?? new Sink(trapezoid);
    }

    public override Sink Locate(Edge edge)
    {
        return this;
    }
}
