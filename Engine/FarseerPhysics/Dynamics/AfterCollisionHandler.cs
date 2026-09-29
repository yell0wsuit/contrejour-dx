using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics;

public delegate void AfterCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse);
