using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics
{
    public delegate bool OnCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact);
}
