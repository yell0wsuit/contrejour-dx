using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class BackgroundBase(Node node, Hashtable config, ContreJourDXGame game) : IUpdatable
    {
        protected Node Node { get; set; } = node;

        protected Hashtable Config { get; set; } = config;

        protected ContreJourDXGame Game { get; set; } = game;

        public virtual void Update(float time)
        {
        }
    }
}
