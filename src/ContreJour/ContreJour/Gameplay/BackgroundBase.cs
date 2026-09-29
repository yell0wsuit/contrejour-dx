using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class BackgroundBase(Node node, Hashtable config, ContreJourGame game) : IUpdatable
{
    protected Node Node { get; set; } = node;

    protected Hashtable Config { get; set; } = config;

    protected ContreJourGame Game { get; set; } = game;

    public virtual void Update(float time)
    {
    }
}
