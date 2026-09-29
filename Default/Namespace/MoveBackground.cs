using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class MoveBackground : BackgroundBase
{
    private Vector2 moveOffset;

    public MoveBackground(Node node, Hashtable config, ContreJourGame game)
        : base(node, config, game)
    {
        if (config.Exists("moveOffset"))
        {
            moveOffset = config.Exists("moveOffset") ? GraphUtil.StringToVector(config.GetString("moveOffset")) : Vector2.Zero;
            if (this.Game.CanShowIntro)
            {
                _ = node.MoveTo(60f, node.Position + moveOffset);
            }
            else
            {
                node.Position += moveOffset;
            }
        }
    }
}
