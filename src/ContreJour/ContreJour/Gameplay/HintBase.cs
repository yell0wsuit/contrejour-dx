using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using ContreJour.Utils;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace ContreJour.Gameplay
{
    public class HintBase : BodyClip, IRemovable
    {
        protected ContreJourGame ContreJour { get; set; }

        public virtual bool ShouldRemove => false;

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public HintBase(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, null, clip, config)
        {
            ContreJour = builder.ContreJour;
            builder.Game.AddUpdatable(this);
            ContreJour.AddTextureToUnload(clip.Texture.Name);
            AddText(Config.GetHashtable("textData"));
            int num = 0;
            while (Config.Exists("textData" + num))
            {
                Hashtable hashtable = Config.GetHashtable("textData" + num);
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
            Clip.AddChild(label);
        }
    }
}
