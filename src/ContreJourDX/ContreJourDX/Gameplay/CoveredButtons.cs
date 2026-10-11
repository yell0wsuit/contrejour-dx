using System;
using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using Mokus2D.Data;
using Mokus2D.Integration.Farseer.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    // Fades the in-game corner buttons while something the player can touch is under them. A faded button lets a
    // tap through to the level and fires only after a 3-second hold, sweeping back in round from the top as the hold goes on. A finger
    // that moves is dragging what it grabbed and drops the hold; one that holds still fires, and the level lets go.
    public class CoveredButtons(ContreJourDXGame game) : Node
    {
        private const float HoldTime = 3f;

        private const float FadedOpacity = 0.25f;

        private const float FadeSeconds = 0.3f;

        // The end of a hold throbs, dipping in brightness this many times, so it is seen to be about to fire.
        private const float PulseSeconds = 0.5f;

        private const int Pulses = 2;

        private const float PulseDepth = 0.5f;

        // Points sampled per side of the overlap between a body and a button.
        private const int Samples = 4;

        private readonly ContreJourDXGame game = game;

        private readonly List<(Button Button, Node Holder)> buttons = [];

        private readonly AABBQuery query = new();

        // The node to add in the button's place; its opacity is the fade, leaving the button's own to its owner.
        public Node Hold(Button button)
        {
            Node holder = new();
            holder.AddChild(button);
            button.HoldCompleted += game.ReleaseTouch;
            buttons.Add((button, holder));
            return holder;
        }

        public override void Update(float time)
        {
            base.Update(time);
            foreach ((Button button, Node holder) in buttons)
            {
                bool covered = button.Visible && IsCovered(button);
                button.HoldTime = covered ? HoldTime : 0f;
                bool holding = button.HoldingTouch != null;
                // The button stays faded while the hold sweeps it back to full, round from the top.
                button.ShowHoldReveal(button.HoldProgress, holding ? Throb(button) : 1f);
                holder.OpacityFloat = holding
                    ? FadedOpacity
                    : Maths.StepTo(holder.OpacityFloat, covered ? FadedOpacity : 1f, time / FadeSeconds);
            }
        }

        private static float Throb(Button button)
        {
            float remaining = button.HoldRemaining;
            if (remaining >= PulseSeconds)
            {
                return 1f;
            }
            // Each dip returns to full, so the last one ends as the button fires.
            float wave = MathF.Sin((PulseSeconds - remaining) / PulseSeconds * Pulses * MathF.PI);
            return 1f - (PulseDepth * wave * wave);
        }

        private bool IsCovered(Button button)
        {
            World world = game.Builder?.World;
            if (world == null)
            {
                return false;
            }
            RectangleFloat bounds = button.Bounds;
            AABB area = new()
            {
                LowerBound = new Vector2(float.MaxValue),
                UpperBound = new Vector2(float.MinValue)
            };
            foreach (Vector2 corner in (ReadOnlySpan<Vector2>)[
                new(bounds.X, bounds.Y),
                new(bounds.X + bounds.Width, bounds.Y),
                new(bounds.X, bounds.Y + bounds.Height),
                new(bounds.X + bounds.Width, bounds.Y + bounds.Height)])
            {
                Vector2 point = game.Builder.ToVec(game.GameRoot.GlobalToLocal(button.LocalToGlobal(corner)));
                area.LowerBound = Vector2.Min(area.LowerBound, point);
                area.UpperBound = Vector2.Max(area.UpperBound, point);
            }
            query.Fixtures.Clear();
            world.QueryAABB(query.CallbackReportFixture, ref area);
            foreach (Fixture fixture in query.Fixtures)
            {
                if (IsInteractive(fixture.Body.UserData) && Overlaps(fixture, area))
                {
                    return true;
                }
            }
            return false;
        }

        // Petit, the lights, and everything that takes a touch: snots and their eyes, springs, draggable ground.
        private static bool IsInteractive(object owner)
        {
            return owner is IClickable or HeroBodyClip or EnergyBodyClip;
        }

        // Large shapes such as ground have boxes far bigger than themselves, so the shape is sampled over the overlap.
        private static bool Overlaps(Fixture fixture, AABB area)
        {
            fixture.GetAABB(out AABB box, 0);
            Vector2 lower = Vector2.Max(box.LowerBound, area.LowerBound);
            Vector2 upper = Vector2.Min(box.UpperBound, area.UpperBound);
            if (lower.X > upper.X || lower.Y > upper.Y)
            {
                return false;
            }
            for (int i = 0; i < Samples; i++)
            {
                for (int j = 0; j < Samples; j++)
                {
                    Vector2 point = lower + ((upper - lower) * new Vector2((i + 0.5f) / Samples, (j + 0.5f) / Samples));
                    if (fixture.TestPoint(ref point))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
