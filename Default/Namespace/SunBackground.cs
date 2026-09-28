using Mokus2D.Visual;

namespace Default.Namespace;

public class SunBackground : MoveBackground
{
    protected float currentOpacity;

    public SunBackground(Node _node, Hashtable _config, ContreJourGame _game)
        : base(_node, _config, _game)
    {
        if (game.CanShowIntro)
        {
            currentOpacity = 255f;
            return;
        }
        currentOpacity = 0f;
        _node.Tweener.Stop();
        game.FlyOpacity = 0f;
    }

    public override void Update(float time)
    {
        if (currentOpacity >= 0f)
        {
            game.FlyOpacity = currentOpacity;
            currentOpacity -= 0.4f;
        }
        else
        {
            game.FlyOpacity = 0f;
        }
    }
}
