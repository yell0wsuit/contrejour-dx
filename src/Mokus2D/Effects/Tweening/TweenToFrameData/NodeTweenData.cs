using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Mokus2D.Effects.Tweening.TweenToFrameData
{
    public struct NodeTweenData(Node node, AnimationFrameData targetFrame)
    {
        public NodeData StartData = new(node);

        public AnimationFrameData TargetFrame = targetFrame;
    }
}
