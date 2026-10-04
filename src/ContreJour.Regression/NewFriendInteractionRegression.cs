using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

using ContreJour.Gameplay;

using FarseerPhysics.Dynamics;

using Mokus2D;
using Mokus2D.Input;
using Mokus2D.PlatformSupport.Input;
using Mokus2D.Util.Data;
using Mokus2D.Visual;
using Mokus2D.Visual.Interfaces;

namespace ContreJour.Regression
{
    internal static class NewFriendInteractionRegression
    {
        private const float TimeStep = 1f / 60f;
        private static readonly MethodInfo TileRectangle = typeof(Sprite).GetMethod("GetTileRectangle", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void VerifyAssets(ContreJourGame game)
        {
            GravityParticleSystem backgroundParticles = (GravityParticleSystem)typeof(ContreJourGame)
                .GetField("particles", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(game);
            ISpriteData particleData = Mokus2DGame.LoadSpriteData("newFriend/McParticle_7");
            Check(backgroundParticles.Particles.Count == 25,
                "New Friend did not create the source's 25 ambient particles.");
            foreach (Particle particle in backgroundParticles.Particles)
            {
                Check(particle.Scale is >= 0.3f and <= 0.7f,
                    "New Friend ambient particles did not use the source's .3-.7 scale range.");
                Check(particle.OpacityFloat >= 76f / 255f && particle.OpacityFloat <= 0.7f && particle.Visible,
                    "New Friend ambient particles did not use the source's .3-.7 opacity range.");
                Check(ReferenceEquals(particle.Texture, particleData.Texture)
                    && particleData.TextureRect.Equals((Rectangle)TileRectangle.Invoke(particle, null)),
                    "New Friend ambient particles did not use the themed atlas frame.");
                GravityParticle falling = (GravityParticle)particle;
                Check(falling.Speed.X < 0f && falling.Speed.Y < 0f,
                    "Valentine particles did not drift left and down after reflecting web Y.");
                Check(Math.Abs(falling.Speed.Length() - (20f * falling.Scale)) < 0.00001f,
                    "Valentine particle speed did not match the source's twenty-times-scale formula.");
                Check(Math.Abs(falling.AngularSpeed) <= float.RadiansToDegrees(0.1f),
                    "Valentine particle angular speed exceeded the source's .1-radian range.");
            }
            Vector2 levelSize = game.LevelSize;
            Check(backgroundParticles.BottomLeftBound == new Vector2(0f, -20f)
                && backgroundParticles.TopRightBound == new Vector2(levelSize.X + 20f, levelSize.Y),
                "Valentine particles did not use the authored world's reflected recycling bounds.");
            GravityParticle recycled = (GravityParticle)backgroundParticles.Particles[0];
            recycled.Position = new Vector2(-1f, levelSize.Y / 2f);
            backgroundParticles.UpdateParticleTime(recycled, 0f);
            Check(recycled.Position.Y == levelSize.Y && recycled.Position.X >= 0f && recycled.Position.X <= levelSize.X,
                "Valentine particles did not respawn along the source's top edge.");
            Check(recycled.Speed.X < 0f && recycled.Speed.Y < 0f
                && Math.Abs(recycled.Speed.Length() - (20f * recycled.Scale)) < 0.00001f,
                "Recycled Valentine particles lost their source motion settings.");
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

        public static void VerifyGroundOutline(ContreJourGame game, Action<float> tick, Action<string> capture = null)
        {
            PlasticineBodyClip ground = game.Plasticine[0];
            PlasticineBorder outline = ground.OutBorder;
            Check(outline is NewFriendPlasticineBorder && outline.Layer == -3 && outline.OpacityFloat == 0f,
                "New Friend ground did not create the hidden yellow rest outline behind the level.");
            // Drag an upward-facing part away from both characters, as a player would.
            PlasticineItem item = ground.FirstItem;
            PlasticineItem chosen = null;
            do
            {
                Vector2 surface = item.GetSurfaceCenterVec();
                if (Vector2.Normalize(item.BodyClip.Normal).Y > 0.7f && Vector2.Distance(surface, game.Hero.Body.Position) > 3f
                    && Vector2.Distance(surface, game.Amie.Body.Position) > 3f)
                {
                    chosen = item;
                    break;
                }
                item = item.NextItem;
            }
            while (item != ground.FirstItem);
            Check(chosen != null, "No draggable ground part was found for the outline check.");
            Vector2 start = chosen.GetSurfaceCenterVec();
            Touch drag = TouchAt(game, start, 7101);
            Check(game.TouchBegin(drag) && chosen.BodyClip.Dragging, "The ground outline check could not drag the ground.");
            for (int frame = 1; frame <= 30; frame++)
            {
                Vector2 target = start + (Vector2.Normalize(chosen.BodyClip.Normal) * (frame / 30f));
                drag.Position = game.Builder.GameRoot.LocalToGlobal(game.Builder.ToPoint(target));
                _ = game.TouchMove(drag);
                tick(TimeStep);
            }
            Check(Math.Abs(outline.OpacityFloat - (102f / 255f)) < 0.0001f,
                "Dragging the ground did not fade the rest outline in to the source's 102 opacity.");
            capture?.Invoke("ground-outline");
            game.TouchEnd(drag);
            Step(tick, 110);
            Check(outline.OpacityFloat > 0f, "The rest outline faded before the source's two-second hold.");
            Step(tick, 40);
            Check(outline.OpacityFloat == 0f, "The rest outline did not fade out after the drag ended.");
        }

        public static void Run(ContreJourGame game, Action<float> tick, Action<string> capture = null)
        {
            VerifyGroundOutline(game, tick, capture);
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

                // A touch that began on nothing releases the balloon when it slides onto it.
                Step(tick, 40);
                amie.LinkToHero();
                Check(amie.Linked, "Relink before the free touch check failed.");
                Touch swipe = TouchAt(game, new Vector2(-10f, -10f), 7003);
                _ = game.TouchBegin(swipe);
                Check(amie.Linked, "A touch far from the companion released it.");
                swipe.Position = game.Builder.GameRoot.LocalToGlobal(game.Builder.ToPoint(amie.Body.Position));
                _ = game.TouchMove(swipe);
                game.TouchEnd(swipe);
                tick(TimeStep);
                Check(!amie.Linked && amie.SnotEnabled, "A free touch sliding onto the inflated companion did not release it.");

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

                Vector2 deathPosition = amie.Clip.Position;
                amie.Explode();
                Step(LifecycleTick, 1);
                Check(!amie.Body.Enabled, "Spike death left Amie's solid physics body active.");
                Check(amie.Clip.Scale is >= 1.02f and <= 1.04f,
                    "Amie's first death shake did not use the web's additive .02-.04 scale step.");
                Step(LifecycleTick, 38);
                Check(amie.Clip.Visible && amie.Clip.Position == deathPosition,
                    "Amie's forty-update death shake ended early or translated her body.");
                Step(LifecycleTick, 1);
                Check(amie.Clip.Visible, "Amie disappeared before all forty death shakes finished.");
                Step(LifecycleTick, 1);
                Check(!amie.Clip.Visible && !amie.Tail.Visible, $"Companion explosion did not hide its body and tail (body={amie.Clip.Visible}, tail={amie.Tail.Visible}, scale={amie.Clip.Scale}).");
                Step(LifecycleTick, 180);
                Check(amie.Clip.Visible && amie.Tail.Visible && amie.Body.BodyType == BodyType.Dynamic, "Companion death did not restart and respawn it.");

                // Restart during the shake must cancel the old pin/explosion lifecycle.
                amie.Explode();
                game.SoftRestart();
                Step(LifecycleTick, 120);
                Check(amie.Body.Enabled && amie.Clip.Visible && amie.Clip.Scale > 0.99f && amie.CanDie(),
                    "An interrupted spike explosion disabled or hid respawned Amie.");

                amie.EatSpeedPauseScaleTime(amie.Body.Position + Vector2.UnitX, 0.5f, 1.3f, 0f, 0.2f);
                Check(!amie.Body.Enabled && !amie.CanDie(), "Eating did not deactivate the companion and block further hazards.");
                VerifyConsumptionRestart(game, LifecycleTick);

                // The finish lifecycle must stop the companion from restarting a won level.
                amie.MarkLevelCompleted();
                primaryHero.Body.BodyType = BodyType.Static;
                amie.Body.BodyType = BodyType.Static;
                amie.Body.Position = new Vector2(center.X, -3f);
                Step(LifecycleTick, 120);
                Check(amie.Body.Position.Y < -2f, "A completed companion falling below the level restarted gameplay.");
                amie.EatSpeedPauseScaleTime(amie.Body.Position, 0.5f, 1.3f, 0f, 0.2f);
                float completedTime = game.TotalTime;
                Step(LifecycleTick, 120);
                Check(!amie.Body.Enabled && game.TotalTime > completedTime + 1.9f,
                    "Eating Amie after completion restarted a won level.");
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

        public static void RunFlowers(ContreJourGame game, Action<float> tick)
        {
            VerifyAttachedSpikes(game, tick);
            BaloonBodyClip amie = game.Amie;
            HeroBodyClip hero = game.Hero;
            SpikesFlowerBodyClip[] flowers = [.. game.Builder.World.BodyList
                .Select(body => body.UserData).OfType<SpikesFlowerBodyClip>()];
            Check(flowers.Length >= 2, "The authored flower level did not contain its two flowers.");
            Vector2 center = game.Builder.ToVec(game.LevelSize) / 2f;
            try
            {
                PlaceCharacters(amie, hero, center);
                amie.LinkToHero();
                Step(tick, 15);
                Check(amie.Linked && amie.CanDie() && !amie.CanTeleport(),
                    "Attachment made Amie immune to hazards or enabled attached teleportation.");
                Check(amie.Body.FixtureList.Count(fixture => !fixture.IsSensor) >= 2,
                    "The physical flower regression did not inflate attached Amie.");
                amie.Body.SetTransform(flowers[0].Body.Position, 0f);
                HoldHero(hero, new Vector2(1000f, 1000f));
                game.Builder.World.Step(TimeStep * game.Builder.PhysicsSpeed);
                Check(!amie.Linked && !amie.Tail.Linked && !amie.Body.Enabled,
                    "The flower did not consume attached Amie and release Petit.");
                game.Builder.World.ProcessChanges();
                Check(CountCompanionJoints(game.Builder.World, amie, hero) == 0,
                    "Flower consumption retained an attachment joint.");
                Check(ReferenceEquals(game.Hero, hero) && hero.Body.Enabled,
                    "Eating Amie replaced or deactivated Petit.");
                int repeatedDestruction = 0;
                void OnDestroyed()
                {
                    repeatedDestruction++;
                }
                amie.DestroyEvent.AddListener(OnDestroyed);
                flowers[1].OnCollisionPoint(amie.Body, null);
                amie.DestroyEvent.RemoveListener(OnDestroyed);
                Check(repeatedDestruction == 0, "A second flower consumed Amie again.");
                void ConsumptionTick(float time)
                {
                    HoldHero(hero, center);
                    tick(time);
                }
                VerifyConsumptionRestart(game, ConsumptionTick);
                Console.WriteLine("New Friend flowers passed: physical attached consumption, Petit release, joint cleanup, duplicate guard, automatic restart.");
            }
            finally
            {
                game.SoftRestart();
                for (int frame = 0; frame < 180; frame++)
                {
                    HoldHero(hero, center);
                    tick(TimeStep);
                }
            }
        }

        private static void VerifyConsumptionRestart(ContreJourGame game, Action<float> tick)
        {
            float previousTime = game.TotalTime;
            int restarts = 0;
            for (int frame = 0; frame < 200; frame++)
            {
                tick(TimeStep);
                if (game.TotalTime < previousTime)
                {
                    restarts++;
                }
                previousTime = game.TotalTime;
                if (frame == 59)
                {
                    Check(restarts == 0 && !game.Amie.Body.Enabled && game.Amie.Clip.Scale == 0f,
                        "Flower consumption did not wait for its restart delay.");
                }
            }
            Check(restarts == 1, $"Flower consumption restarted {restarts} times instead of once.");
            BaloonBodyClip amie = game.Amie;
            Check(amie.Body.Enabled && amie.Clip.Visible && amie.Tail.Visible && amie.Clip.Scale > 0.99f
                && amie.Body.BodyType == BodyType.Dynamic && amie.CanDie(),
                "The automatic flower restart did not restore Amie.");
        }

        private static void VerifyAttachedSpikes(ContreJourGame game, Action<float> tick)
        {
            BaloonBodyClip amie = game.Amie;
            HeroBodyClip hero = game.Hero;
            World world = game.Builder.World;
            SimpleSpikesBodyClip spikes = world.BodyList.Select(body => body.UserData).OfType<SimpleSpikesBodyClip>().First();
            Dictionary<Body, bool> enabled = [];
            foreach (Body body in world.BodyList)
            {
                if (body != amie.Body && body != hero.Body && body != amie.Tail.Start && body != amie.Tail.Middle && body != amie.Tail.End)
                {
                    enabled.Add(body, body.Enabled);
                    body.Enabled = false;
                }
            }
            Vector2 center = game.PhysicsLevelSize / 2f;
            try
            {
                PlaceCharacters(amie, hero, center);
                amie.LinkToHero();
                Step(tick, 15); // Allow the actual delayed inflated fixture to appear.
                Check(amie.Linked && amie.Body.FixtureList.Count(fixture => !fixture.IsSensor) >= 2,
                    "The physical spike regression did not inflate attached Amie.");
                Check(amie.CanDie() && !amie.CanTeleport(),
                    "Attached Amie gained spike immunity or lost her teleport restriction.");
                spikes.Body.Enabled = true;
                amie.Body.SetTransform(spikes.Body.Position, 0f);
                HoldHero(hero, new Vector2(1000f, 1000f));
                // Contact callbacks, fixture removal, and joint cleanup run inside the real world step.
                world.Step(TimeStep * game.Builder.PhysicsSpeed);
                world.ProcessChanges();
                Check(!amie.Linked && !amie.Tail.Visible && !amie.CanDie(),
                    "Physical spike contact did not explode attached Amie and release Petit.");
                Check(CountCompanionJoints(world, amie, hero) == 0,
                    "Attached spike death retained a companion joint.");
                HoldHero(hero, center);
                tick(TimeStep);
                Check(!amie.Body.Enabled, "Attached spike death did not deactivate Amie's body.");
                Console.WriteLine("Attached Amie spikes passed: physical contact, explosion, Petit release, joint cleanup.");
            }
            finally
            {
                foreach ((Body body, bool wasEnabled) in enabled)
                {
                    body.Enabled = wasEnabled;
                }
                game.SoftRestart();
                for (int frame = 0; frame < 180; frame++)
                {
                    HoldHero(hero, center);
                    tick(TimeStep);
                }
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
