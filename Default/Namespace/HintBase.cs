using System.Diagnostics.CodeAnalysis;

using ContreJour.Utils;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace Default.Namespace;

public class HintBase : BodyClip, IRemovable
{
    protected ContreJourGame contreJour;

    public virtual bool ShouldRemove => false;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public HintBase(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        contreJour = _builder.ContreJour;
        _builder.Game.AddUpdatable(this);
        contreJour.AddTextureToUnload(_clip.Texture.Name);
        AddText(config.GetHashtable("textData"));
        int num = 0;
        while (config.Exists("textData" + num))
        {
            Hashtable hashtable = config.GetHashtable("textData" + num);
            if (hashtable != null)
            {
                AddText(hashtable);
            }
            num++;
        }
    }

    private void AddText(Hashtable textData)
    {
        Vector2 vector = textData.GetVector("textSize");
        Vector2 vector2 = textData.GetVector("textPosition");
        int num = textData.GetInt("fontSize");
        string text = textData.GetString("textAlign");
        float x = 0.5f;
        if (text == "right")
        {
            x = 1f;
            vector2.X += vector.X / 2f;
        }
        else if (text == "left")
        {
            x = 0f;
            vector2.X -= vector.X / 2f;
        }
        Label label = ContreJourLabelUtil.CreateMultilineLabel(text: textData.GetString("text").Localize(), size: num / 2);
        label.Anchor = new Vector2(x, 0.5f);
        if (textData.Exists("color"))
        {
            label.Color = textData.GetUInt("color").ToRGBColor();
        }
        label.Position = vector2;
        clip.AddChild(label);
    }
}
