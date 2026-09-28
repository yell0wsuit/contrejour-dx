namespace Mokus2D.Integration.Farseer.Construction.Processors;

public abstract class PhysicsProcessor
{
    protected readonly PhysicsConstructor Constructor;

    protected PhysicsProcessor(PhysicsConstructor constructor)
    {
        Constructor = constructor;
    }
}
