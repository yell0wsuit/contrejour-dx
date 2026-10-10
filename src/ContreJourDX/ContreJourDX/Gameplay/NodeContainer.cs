using System;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class NodeContainer(Func<Node> nodeFactory) : Node
    {
        private readonly Func<Node> _nodeFactory = nodeFactory;

        // The scene the factory made, once the container is on stage.
        public Node Content { get; private set; }

        protected override void OnAddedToStage()
        {
            base.OnAddedToStage();
            Content = _nodeFactory();
            AddChild(Content);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Content?.Dispose();
        }
    }
}
