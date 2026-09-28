using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class AlphaForeground : ForegroundBase, IUpdatable
{
    protected CosChanger changer;

    public AlphaForeground(ContreJourLevelBuilder _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        float num = _config.GetFloat("alphaStep");
        changer = new CosChanger(num, num);
        changer.MaxValue = _config.GetFloat("maximumAlpha");
        changer.MinValue = _config.GetFloat("minimumAlpha");
    }

    public override void Update(float time)
    {
        changer.Update(time);
        clip.OpacityFloat = changer.Value;
    }
}
