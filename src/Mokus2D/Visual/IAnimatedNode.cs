using System;

namespace Mokus2D.Visual;

public interface IAnimatedNode
{
    bool Rewind { get; set; }

    bool Repeat { get; set; }

    float Speed { get; set; }

    bool Stoped { get; set; }

    float MinFrame { get; set; }

    float MaxFrame { get; set; }

    int TotalFrames { get; }

    float CurrentFrame { get; set; }

    event Action<IAnimatedNode> EndEvent;

    void GotoAndPlay(float frame);

    void GotoAndStop(float frame);
}
