using System;
using System.Collections.Generic;

using ContreJour.Clips;
using ContreJour.Clips.level1;

using Mokus2D.Util;
using Mokus2D.Visual;

namespace ContreJour.Gameplay.Rose
{
    public class FinalRose : Node, IAnimatedNode
    {
        private readonly List<IAnimatedNode> parts = [];

        private readonly AnimationPlayer player;

        private readonly McRoseHeadDown head;

        private bool updated;

        public bool Rewind
        {
            get => player.Rewind;
            set => player.Rewind = value;
        }

        public bool Repeat
        {
            get => player.Repeat;
            set => player.Repeat = value;
        }

        public float Speed
        {
            get => player.Speed;
            set => player.Speed = value;
        }

        public bool Stoped
        {
            get => player.Stoped;
            set => player.Stoped = value;
        }

        public float MinFrame
        {
            get => player.MinFrame;
            set => player.MinFrame = value;
        }

        public float MaxFrame
        {
            get => player.MaxFrame;
            set => player.MaxFrame = value;
        }

        public int FrameValue => player.FrameValue;

        public int TotalFrames => 79;

        public float CurrentFrame
        {
            get => player.CurrentFrame;
            set => player.CurrentFrame = value;
        }

        public event Action<IAnimatedNode> EndEvent;

        public FinalRose()
        {
            player = new AnimationPlayer(this);
            player.EndEvent += PlayerOnEndEvent;
            AddChildPart(new MovieClip(ClipIds.Level1.McStebloAnimation));
            AddChildPart(new McLystok2());
            head = new McRoseHeadDown();
            AddChildPart(head);
            AddPart(head.content);
            McLystokMain mcLystokMain = new()
            {
                MaxFrame = 79f
            };
            mcLystokMain.tear.RemoveFromParent();
            AddChildPart(mcLystokMain);
            MinFrame = 0.1f;
        }

        private void PlayerOnEndEvent()
        {
            EndEvent.Dispatch(this);
        }

        public override void Update(float time)
        {
            if (!updated)
            {
                head.content.IgnoreAnimations("light");
                updated = true;
            }
            player.Update(time);
            foreach (IAnimatedNode part in parts)
            {
                part.GotoAndStop(part.MaxFrame - CurrentFrame);
            }
            head.content.light.OpacityFloat = CurrentFrame / 30f;
        }

        private void AddChildPart(IAnimatedNode node)
        {
            AddChild((Node)node);
            AddPart(node);
        }

        private void AddPart(IAnimatedNode node)
        {
            node.GotoAndStop(node.MaxFrame);
            parts.Add(node);
            node.Stoped = true;
            node.Repeat = false;
        }

        public void GotoAndPlay(float frame)
        {
            player.GotoAndPlay(frame);
        }

        public void GotoAndStop(float frame)
        {
            player.GotoAndStop(frame);
        }
    }
}
