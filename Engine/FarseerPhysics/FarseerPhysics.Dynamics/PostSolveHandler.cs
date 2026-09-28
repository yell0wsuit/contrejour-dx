using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics;

public delegate void PostSolveHandler(Contact contact, ContactVelocityConstraint impulse);
