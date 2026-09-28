using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BackgroundBase : IUpdatable
{
    protected Node node;

    protected Hashtable config;

    protected ContreJourGame game;

    public BackgroundBase(Node _node, Hashtable _config, ContreJourGame _game)
    {
        config = _config;
        node = _node;
        game = _game;
    }

    public virtual void Update(float time)
    {
    }
}
