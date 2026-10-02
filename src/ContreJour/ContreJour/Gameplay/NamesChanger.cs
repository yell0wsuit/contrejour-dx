using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

using ContreJour.Clips;
using ContreJour.Utils;

using Mokus2D.Graphics;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace ContreJour.Gameplay
{
    public class NamesChanger : Node
    {
        private float currentIndex;

        private readonly List<Node> names = [];

        public float CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (currentIndex == value)
                {
                    return;
                }
                foreach (Node name in names)
                {
                    name.Visible = false;
                }
                currentIndex = value;
                float num = currentIndex % names.Count;
                if (num < 0f)
                {
                    num += names.Count;
                }
                int num2 = (int)num % names.Count;
                int num3 = (num2 + 1) % names.Count;
                float num4 = currentIndex % 1f;
                if (num4 < 0f)
                {
                    num4 += 1f;
                }
                names[(num3 + 1) % names.Count].Visible = false;
                Node node = names[num2];
                Node node2 = names[num3];
                node.Position = new Vector2((0f - num4) * 1300f, 0f);
                node2.Position = new Vector2((1f - num4) * 1300f, 0f);
                node.Visible = true;
                node2.Visible = Maths.FuzzyNotEquals(num4, 0f);
            }
        }

        public NamesChanger(float scale)
        {
            for (int i = 0; i < Constants.ChaptersCount; i++)
            {
                Node node = CreateChapterName(i);
                names.Add(node);
                node.Scale = scale;
            }
            if (Constants.IsTrial)
            {
                Node node2;
                if (ContreJourLabelUtil.IsEnglish)
                {
                    node2 = new Sprite(ClipIds.Menu.McChapterMoreName);
                }
                else
                {
                    node2 = CreateLabelColor("GET_MORE_LEVELS", new Color(133, 213, 255));
                    node2.Scale = 0.8f;
                    foreach (Node child in node2.Children)
                    {
                        child.X += 20f;
                    }
                }
                names.Add(node2);
            }
            foreach (Node name in names)
            {
                AddChild(name);
                name.Scale *= 0.9f;
                name.Scale *= 0.9375f;
                name.Visible = false;
            }
            names[1].Color = Color.White * 0.8f;
            if (Constants.IsTrial)
            {
                names[2].Color = names[1].Color;
            }
            currentIndex = -1f;
            CurrentIndex = 0f;
        }

        private static Node CreateChapterName(int index)
        {
            return ContreJourLabelUtil.IsEnglish
                ? new Sprite(string.Format(CultureInfo.InvariantCulture, "menu/McChapter{0}Name", index + 1))
                : CreateLabelColor(color: index switch
                {
                    3 => ContreJourConstants.WhiteLightColor * 1.8f,
                    1 => ContreJourConstants.BlueLightColor * 1.8f,
                    5 => ContreJourConstants.GreenLightColor,
                    _ => Color.Black,
                }, text: string.Format(CultureInfo.InvariantCulture, "CHAPTER{0}", index + 1));
        }

        private static Node CreateLabelColor(string text, Color color)
        {
            Node node = new();
            Label label = ContreJourLabelUtil.CreateMultilineLabel(32f, text);
            label.Color = color;
            Label label2 = ContreJourLabelUtil.CreateMultilineLabel(32f, text);
            label2.Color = Color.Black;
            label2.OpacityByte = 80;
            label2.Position = new Vector2(3f, -3f);
            label.AnchorY = label2.AnchorY = 0.5f;
            node.AddChild(label2);
            node.AddChild(label);
            return node;
        }
    }
}
