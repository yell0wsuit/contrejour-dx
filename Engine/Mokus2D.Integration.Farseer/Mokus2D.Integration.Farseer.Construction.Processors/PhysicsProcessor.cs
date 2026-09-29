namespace Mokus2D.Integration.Farseer.Construction.Processors;

public abstract class PhysicsProcessor(PhysicsConstructor constructor)
{
    protected PhysicsConstructor Constructor { get; } = constructor;
}
