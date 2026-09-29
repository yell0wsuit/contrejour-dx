using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics;

public delegate void PreSolveHandler(Contact contact, ref Manifold oldManifold);
