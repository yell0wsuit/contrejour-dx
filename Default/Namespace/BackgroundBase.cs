using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BackgroundBase(Node _node, Hashtable _config, ContreJourGame _game) : IUpdatable
{
    protected Node Node { get; set; } = _node;

    protected Hashtable Config { get; set; } = _config;

    protected ContreJourGame Game { get; set; } = _game;

    public virtual void Update(float time)
    {
    }
}
