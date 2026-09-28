using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class MoveBackground : BackgroundBase
{
    protected Vector2 moveOffset;

    public MoveBackground(Node _node, Hashtable _config, ContreJourGame _game)
        : base(_node, _config, _game)
    {
        if (_config.Exists("moveOffset"))
        {
            moveOffset = _config.Exists("moveOffset") ? GraphUtil.StringToVector(_config.GetString("moveOffset")) : Vector2.Zero;
            if (game.CanShowIntro)
            {
                _ = _node.MoveTo(60f, _node.Position + moveOffset);
            }
            else
            {
                _node.Position += moveOffset;
            }
        }
    }
}
