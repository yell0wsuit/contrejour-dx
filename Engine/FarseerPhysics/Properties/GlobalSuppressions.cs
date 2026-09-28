using System.Diagnostics.CodeAnalysis;

// The world owns a breakable body's bodies: World.RemoveBreakableBody removes them.
[assembly: SuppressMessage("Design", "CA1001", Scope = "type", Target = "~T:FarseerPhysics.Dynamics.BreakableBody", Justification = "The World owns and removes the bodies.")]
