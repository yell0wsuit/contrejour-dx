using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BackgroundBase(Node _node, Hashtable _config, ContreJourGame _game) : IUpdatable
{
    protected Node node = _node;

    protected Hashtable config = _config;

    protected ContreJourGame game = _game;

    public virtual void Update(float time)
    {
    }
}
