using System;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace Mokus2D.Integration.Farseer.Physics;

public class ContactListener
{
    public ContactListener(World world)
    {
        ContactManager contactManager = world.ContactManager;
        contactManager.BeginContact = (BeginContactDelegate)Delegate.Combine(contactManager.BeginContact, new BeginContactDelegate(BeginContact));
        ContactManager contactManager2 = world.ContactManager;
        contactManager2.EndContact = (EndContactDelegate)Delegate.Combine(contactManager2.EndContact, new EndContactDelegate(EndContact));
        ContactManager contactManager3 = world.ContactManager;
        contactManager3.PreSolve = (PreSolveDelegate)Delegate.Combine(contactManager3.PreSolve, new PreSolveDelegate(PreSolve));
        ContactManager contactManager4 = world.ContactManager;
        contactManager4.PostSolve = (PostSolveDelegate)Delegate.Combine(contactManager4.PostSolve, new PostSolveDelegate(PostSolve));
    }

    public bool BeginContact(Contact contact)
    {
        BodyClip bodyClip = contact.FixtureA.Body.UserData as BodyClip;
        BodyClip bodyClip2 = contact.FixtureB.Body.UserData as BodyClip;
        bodyClip?.OnCollisionStart(contact.FixtureB, contact);
        bodyClip2?.OnCollisionStart(contact.FixtureA, contact);
        return true;
    }

    public void EndContact(Contact contact)
    {
        BodyClip bodyClip = contact.FixtureA.Body.UserData as BodyClip;
        BodyClip bodyClip2 = contact.FixtureB.Body.UserData as BodyClip;
        bodyClip?.OnCollisionEnd(contact.FixtureB, contact);
        bodyClip2?.OnCollisionEnd(contact.FixtureA, contact);
    }

    public void PreSolve(Contact contact, ref Manifold manifold)
    {
        if (contact.IsTouching)
        {
            BodyClip bodyClip = contact.FixtureA.Body.UserData as BodyClip;
            BodyClip bodyClip2 = contact.FixtureB.Body.UserData as BodyClip;
            bodyClip?.OnCollision(contact.FixtureB, contact);
            bodyClip2?.OnCollision(contact.FixtureA, contact);
        }
    }

    public void PostSolve(Contact contact, ContactVelocityConstraint impulse)
    {
        if (contact.IsTouching)
        {
            BodyClip bodyClip = contact.FixtureA.Body.UserData as BodyClip;
            BodyClip bodyClip2 = contact.FixtureB.Body.UserData as BodyClip;
            bodyClip?.PostSolvePointImpulse(contact.FixtureB, contact, impulse);
            bodyClip2?.PostSolvePointImpulse(contact.FixtureA, contact, impulse);
        }
    }
}
