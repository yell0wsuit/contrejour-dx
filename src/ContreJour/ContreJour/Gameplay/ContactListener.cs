using System;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace ContreJour.Gameplay
{
    public class ContactListener
    {
        public ContactListener(World world)
        {
            //IL_0019: Unknown result type (might be due to invalid IL or missing references)
            //IL_0023: Expected O, but got Unknown
            //IL_0023: Unknown result type (might be due to invalid IL or missing references)
            //IL_002d: Expected O, but got Unknown
            //IL_0040: Unknown result type (might be due to invalid IL or missing references)
            //IL_004a: Expected O, but got Unknown
            //IL_004a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0054: Expected O, but got Unknown
            //IL_0067: Unknown result type (might be due to invalid IL or missing references)
            //IL_0071: Expected O, but got Unknown
            //IL_0071: Unknown result type (might be due to invalid IL or missing references)
            //IL_007b: Expected O, but got Unknown
            //IL_008e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0098: Expected O, but got Unknown
            //IL_0098: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a2: Expected O, but got Unknown
            ContactManager contactManager = world.ContactManager;
            contactManager.BeginContact = (BeginContactHandler)Delegate.Combine((Delegate)(object)contactManager.BeginContact, new BeginContactHandler(BeginContact));
            ContactManager contactManager2 = world.ContactManager;
            contactManager2.EndContact = (EndContactHandler)Delegate.Combine((Delegate)(object)contactManager2.EndContact, new EndContactHandler(EndContact));
            ContactManager contactManager3 = world.ContactManager;
            contactManager3.PreSolve = (PreSolveHandler)Delegate.Combine((Delegate)(object)contactManager3.PreSolve, new PreSolveHandler(PreSolve));
            ContactManager contactManager4 = world.ContactManager;
            contactManager4.PostSolve = (PostSolveHandler)Delegate.Combine((Delegate)(object)contactManager4.PostSolve, new PostSolveHandler(PostSolve));
        }

        public static bool BeginContact(Contact contact)
        {
            ProcessContact(contact, delegate (BodyClip bodyClip, Body body)
            {
                bodyClip.OnCollisionStartPoint(body, contact);
            });
            return true;
        }

        private static void ProcessContact(Contact contact, Action<BodyClip, Body> action)
        {
            BodyClip bodyClip = (BodyClip)contact.FixtureA.Body.UserData;
            BodyClip bodyClip2 = (BodyClip)contact.FixtureB.Body.UserData;
            Body body = contact.FixtureA.Body;
            Body body2 = contact.FixtureB.Body;
            if (bodyClip != null)
            {
                action(bodyClip, body2);
            }
            if (bodyClip2 != null)
            {
                action(bodyClip2, body);
            }
        }

        public static void EndContact(Contact contact)
        {
            ProcessContact(contact, delegate (BodyClip bodyClip, Body body)
            {
                bodyClip.OnCollisionEndPoint(body, contact);
            });
        }

        public static void PreSolve(Contact contact, ref Manifold manifold)
        {
            if (contact.IsTouching)
            {
                ProcessContact(contact, delegate (BodyClip bodyClip, Body body)
                {
                    bodyClip.OnCollisionPoint(body, contact);
                });
            }
        }

        public static void PostSolve(Contact contact, ContactVelocityConstraint impulse)
        {
            if (contact.IsTouching)
            {
                BodyClip bodyClip = (BodyClip)contact.FixtureA.Body.UserData;
                BodyClip bodyClip2 = (BodyClip)contact.FixtureB.Body.UserData;
                bodyClip?.PostSolvePointImpulse(contact.FixtureB.Body, contact, impulse);
                bodyClip2?.PostSolvePointImpulse(contact.FixtureA.Body, contact, impulse);
            }
        }
    }
}
