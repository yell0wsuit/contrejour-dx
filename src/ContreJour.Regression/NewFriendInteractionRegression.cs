using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

using ContreJour.Gameplay;

using FarseerPhysics.Dynamics;

using Mokus2D.Input;
using Mokus2D;
using Mokus2D.Visual;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Util.Data;
using Mokus2D.PlatformSupport.Input;

namespace ContreJour.Regression
{
    internal static class NewFriendInteractionRegression
    {
        private const float TimeStep = 1f / 60f;
        private static readonly MethodInfo TileRectangle = typeof(Sprite).GetMethod("GetTileRectangle", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void VerifyAssets(ContreJourGame game)
        {
            HashSet<BodyClip> checkedClips = [];
            foreach (Body body in game.Builder.World.BodyList)
            {
                if (body.UserData is not BodyClip clip || !checkedClips.Add(clip))
                {
                    continue;
                }
                if (clip is SimpleSpikesBodyClip && clip.Config.GetString("viewType").Contains("Circle"))
                {
                    IMovieClipData expected = Mokus2DGame.LoadMovieClipData("newFriend/McCircleSpikesView_7");
                    Check(clip.Clip is Sprite sprite && ReferenceEquals(sprite.Texture, expected.Texture)
                        && expected.Frames.Any(frame => frame.Rect.Equals((Rectangle)TileRectangle.Invoke(sprite, null))),
                        "Circle spikes did not use the exact themed atlas frames.");
                }
                if (clip is DragableBodyClip)
                {
                    string texture = clip is RoundDragBodyClip ? "newFriend/McRoundDragView_7" : "newFriend/McDragView_7";
                    ISpriteData expected = Mokus2DGame.LoadSpriteData(texture);
                    Check(clip.Clip is Sprite sprite && ReferenceEquals(sprite.Texture, expected.Texture)
                        && expected.TextureRect.Equals((Rectangle)TileRectangle.Invoke(sprite, null)),
                        "Drag handle did not use the exact themed atlas frame.");
                }
            }
        }

        public static void Run(ContreJourGame game, Action<float> tick, Action<string> capture = null)
        {
            BaloonBodyClip amie = game.Amie ?? throw new InvalidOperationException("New Friend companion was not constructed.");
            Check(game.Builder.PhysicsSpeed == 1.2f && FarseerPhysics.Settings.VelocityIterations == 16,
                "New Friend did not use the web physics speed and solver iterations.");
            Check(FarseerPhysics.Settings.ContinuousPhysics, "New Friend did not enable continuous physics as the source does.");
            Check(amie.Clip.Layer == 9, "Amie did not use the web companion layer below Petit and above snot.");
            Check(amie.Clip.Children[^2] is BaloonTailSprite && amie.Clip.Children[^1] is HeroEye,
                "Companion tail did not draw after the legs and before the eye, as in the web game.");
            HeroBodyClip primaryHero = game.Hero;
            World world = game.Builder.World;
            Dictionary<Body, bool> enabled = [];
            // Retain the real level, camera, input dispatcher, gravity, bodies, and tail joints,
            // while removing obstacle contacts from this focused attachment/lift measurement.
            foreach (Body body in world.BodyList)
            {
                if (body != amie.Body && body != primaryHero.Body && body != amie.Tail.Start && body != amie.Tail.Middle && body != amie.Tail.End)
                {
                    enabled.Add(body, body.Enabled);
                    body.Enabled = false;
                }
            }
            try
            {
                // Check the actual builder integration, including long-frame
                // clamping and force clearing, with an isolated dynamic body.
                Body probe = world.CreateCircle(0.1f, new Vector2(1000f, 1000f), density: 1f, dynamic: true);
                probe.SetSensor(true);
                world.ProcessChanges();
                probe.ApplyForce(new Vector2(0f, probe.Mass * 20f));
                game.Builder.Update(0.1f);
                Check(Math.Abs(probe.LinearVelocity.Y - 0.48f) < 0.00001f,
                    "Source single-step integration did not preserve the full applied force at min(delta,.04)*1.2.");
                world.RemoveBody(probe);
                world.ProcessChanges();
                Vector2 center = game.Builder.ToVec(game.LevelSize) / 2f;
                PlaceCharacters(amie, primaryHero, center);
                Touch drag = TouchAt(game, amie.Tail.End.Position, 7001);
                Check(game.TouchBegin(drag), "Game rejected companion tail touch.");
                Check(amie.Tail.Dragging, "Touch dispatcher did not select the companion tail tip.");
                drag.Position = game.Builder.GameRoot.LocalToGlobal(game.Builder.ToPoint(primaryHero.Body.Position));
                _ = game.TouchMove(drag);
                tick(TimeStep);
                Check(amie.Linked, "Dragging the tail onto the primary hero did not attach it.");
                Check(ReferenceEquals(game.Hero, primaryHero), "Companion attachment replaced the primary hero.");
                Check(!amie.SnotEnabled && amie.Tail.Linked, "Attachment did not update snot/tail state.");
                Check(amie.Tail.End.BodyType == BodyType.Dynamic, "Attached tail endpoint is not dynamic.");
                game.TouchEnd(drag);
                Check(!amie.Tail.Dragging, "Ending the drag left its touch state active.");

                float beforeLift = amie.Body.Position.Y;
                Console.WriteLine($"lift before companion={amie.Body.Position} v={amie.Body.LinearVelocity} mass={amie.Body.Mass} type={amie.Body.BodyType} enabled={amie.Body.Enabled}; hero={primaryHero.Body.Position} mass={primaryHero.Body.Mass} type={primaryHero.Body.BodyType}; gravity={world.Gravity}");
                Step(tick, 60);
                Console.WriteLine($"lift after companion={amie.Body.Position} v={amie.Body.LinearVelocity} mass={amie.Body.Mass} type={amie.Body.BodyType} enabled={amie.Body.Enabled} linked={amie.Linked}; hero={primaryHero.Body.Position} mass={primaryHero.Body.Mass}");
                Check(amie.Body.Position.Y > beforeLift + 0.25f, "Attached companion did not lift against DX's downward gravity.");
                Check(amie.Body.LinearVelocity.Y > 0f, "Companion lift velocity has the wrong Y sign.");

                Touch release = TouchAt(game, amie.Body.Position, 7002);
                _ = game.TouchBegin(release);
                game.TouchEnd(release);
                tick(TimeStep);
                Check(!amie.Linked && amie.SnotEnabled && !amie.Tail.Linked, "Clicking the inflated companion did not release the hero.");
                Check(amie.Tail.End.BodyType == BodyType.Kinematic, "Released tail endpoint did not return to kinematic mode.");
                Check(CountCompanionJoints(world, amie, primaryHero) == 0, "Release retained an attachment joint.");

                amie.LinkToHero();
                Check(amie.Linked, "Explicit relink failed after release.");
                primaryHero.TeleportEvent.SendEvent();
                tick(TimeStep);
                Check(!amie.Linked && amie.SnotEnabled, "Primary hero teleport/destruction did not release the companion.");
                Check(CountCompanionJoints(world, amie, primaryHero) == 0, "Hero teleport retained an attachment joint.");

                // Lifecycle assertions use the authored terrain. The low companion spawn needs
                // its real support, and the primary hero must not fail or finish during these checks.
                foreach ((Body body, bool wasEnabled) in enabled)
                {
                    body.Enabled = wasEnabled;
                }
                Vector2 heldHeroPosition = center + new Vector2(0f, 5f);
                void LifecycleTick(float time)
                {
                    HoldHero(primaryHero, heldHeroPosition);
                    tick(time);
                    HoldHero(primaryHero, heldHeroPosition);
                }

                amie.Restart();
                Step(LifecycleTick, 15);
                // Restart fades the old body for .2s, then the source spawn
                // sequence waits another .2s before bringing up the light.
                Step(LifecycleTick, 12);
                Check(amie.SpawnPortal.Particles[0].TextureSize == new Vector2(86f, 86f),
                    "Amie's spawn light did not use the source 86-pixel glow texture.");
                Check(amie.SpawnPortal.Layer == 8 && amie.SpawnPortal.Visible && amie.SpawnPortal.ItemsScale > 0f,
                    $"Amie's spawn light did not appear behind her during respawn: layer={amie.SpawnPortal.Layer}, visible={amie.SpawnPortal.Visible}, scale={amie.SpawnPortal.ItemsScale}, target={amie.SpawnPortal.TargetScale}, first={amie.SpawnPortal.Particles[0].Position}.");
                Vector2 spawnPixels = game.Builder.ToPoint(amie.SpawnPosition);
                Vector2 lightCenter = Vector2.Zero;
                foreach (Particle particle in amie.SpawnPortal.Particles)
                {
                    lightCenter += particle.Position;
                }
                lightCenter /= amie.SpawnPortal.Particles.Count;
                Check(Vector2.Distance(lightCenter, spawnPixels) < 30f,
                    $"Amie's spawn light orbited the wrong position: light={lightCenter}, spawn={spawnPixels}.");
                capture?.Invoke("amie-spawn-light");
                Check(amie.Clip.Layer == 9, "Companion respawn did not restore its drawing layer.");
                Check(Vector2.Distance(amie.Body.Position, amie.SpawnPosition) < 0.001f, "Companion restart did not restore the authored spawn position.");
                Check(amie.Clip.Visible && amie.Tail.Visible, "Companion restart did not restore its body and tail visibility.");
                // Restart again while the previous portal/scale callbacks are pending.
                amie.Restart();
                Step(LifecycleTick, 66);
                Check(amie.Body.BodyType == BodyType.Static, "A stale respawn callback activated the companion before its new spawn animation finished.");
                capture?.Invoke("amie-spawn-light-full");
                Step(LifecycleTick, 20);
                Check(amie.Body.BodyType == BodyType.Dynamic && amie.Clip.Scale > 0.99f, "Companion respawn did not restore an active full-size body.");
                float fixtureMass = 0f;
                foreach (Fixture fixture in amie.Body.FixtureList)
                {
                    fixtureMass += fixture.Shape.MassData.Mass;
                }
                Check(Math.Abs(amie.Body.Mass - fixtureMass) < 0.00001f, "Companion respawn overrode the source fixture-derived mass.");
                Step(LifecycleTick, 45);
                Check(!amie.SpawnPortal.Visible && amie.SpawnPortal.ItemsScale == 0f,
                    "Amie's spawn light did not fade away after respawn.");

                amie.Explode();
                Step(LifecycleTick, 45);
                Check(!amie.Clip.Visible && !amie.Tail.Visible, $"Companion explosion did not hide its body and tail (body={amie.Clip.Visible}, tail={amie.Tail.Visible}, scale={amie.Clip.Scale}).");
                Step(LifecycleTick, 180);
                Check(amie.Clip.Visible && amie.Tail.Visible && amie.Body.BodyType == BodyType.Dynamic, "Companion death did not restart and respawn it.");

                amie.EatSpeedPauseScaleTime(amie.Body.Position + Vector2.UnitX, 0.5f, 1.3f, 0f, 0.2f);
                Check(!amie.Body.Enabled && !amie.CanDie(), "Eating did not deactivate the companion and block further hazards.");
                Step(LifecycleTick, 200);
                Check(amie.Body.Enabled && amie.Clip.Visible && amie.Tail.Visible && amie.Body.BodyType == BodyType.Dynamic, "Companion eating did not restart and respawn it.");

                // The finish lifecycle must stop the companion from restarting a won level.
                amie.MarkLevelCompleted();
                primaryHero.Body.BodyType = BodyType.Static;
                amie.Body.BodyType = BodyType.Static;
                amie.Body.Position = new Vector2(center.X, -3f);
                Step(LifecycleTick, 120);
                Check(amie.Body.Position.Y < -2f, "A completed companion falling below the level restarted gameplay.");
                Console.WriteLine("New Friend interactions passed: tail input, attach, upward lift, click release, teleport release, respawn, spawn callback cancellation, death, eating, completion guard.");
            }
            finally
            {
                foreach ((Body body, bool wasEnabled) in enabled)
                {
                    body.Enabled = wasEnabled;
                }
                game.SoftRestart();
                Step(tick, 180);
            }
        }

        private static void PlaceCharacters(BaloonBodyClip amie, HeroBodyClip hero, Vector2 center)
        {
            amie.Body.BodyType = hero.Body.BodyType = BodyType.Dynamic;
            amie.Body.Enabled = hero.Body.Enabled = true;
            amie.Body.SetTransform(center, 0f);
            hero.Body.SetTransform(center - new Vector2(0f, 1.2f), 0f);
            amie.Body.LinearVelocity = hero.Body.LinearVelocity = Vector2.Zero;
            amie.Body.AngularVelocity = hero.Body.AngularVelocity = 0f;
            amie.Tail.SetPositions(center);
            amie.Tail.End.Position = hero.Body.Position + new Vector2(0.4f, 0f);
            amie.ForceClipPosition();
            hero.ForceClipPosition();
        }

        private static void HoldHero(HeroBodyClip hero, Vector2 position)
        {
            hero.Body.BodyType = BodyType.Static;
            hero.Body.Position = position;
            hero.Body.LinearVelocity = Vector2.Zero;
            hero.Body.AngularVelocity = 0f;
            hero.ForceClipPosition();
        }

        private static Touch TouchAt(ContreJourGame game, Vector2 position, int id)
        {
            Touch touch = new();
            touch.Initialize(new CursorPoint(game.Builder.GameRoot.LocalToGlobal(game.Builder.ToPoint(position)), id, TouchType.Touch));
            return touch;
        }

        private static int CountCompanionJoints(World world, BaloonBodyClip amie, HeroBodyClip hero)
        {
            int count = 0;
            foreach (FarseerPhysics.Dynamics.Joints.Joint joint in world.JointList)
            {
                if ((joint.BodyA == amie.Body && joint.BodyB == amie.Tail.Start) || (joint.BodyB == amie.Body && joint.BodyA == amie.Tail.Start) || (joint.BodyA == hero.Body && joint.BodyB == amie.Tail.End) || (joint.BodyB == hero.Body && joint.BodyA == amie.Tail.End))
                {
                    count++;
                }
            }
            return count;
        }

        private static void Step(Action<float> tick, int frames)
        {
            for (int index = 0; index < frames; index++)
            {
                tick(TimeStep);
            }
        }

        private static void Check(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
