using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;

namespace Mokus2D.Content.Serialization;

public class AnimationDeserializer : GraphicsDeserializerBase<AnimationData>
{
	public override bool UseSuffix => false;

	public AnimationDeserializer(IGraphicsLoader loader)
		: base(loader)
	{
	}

	public override AnimationData Deserialize(string id, XElement element)
	{
		AnimationData animationData = new AnimationData();
		XAttribute xAttribute = element.Attribute("precalculatedBounds");
		if (xAttribute != null)
		{
			animationData.PrecalculatedBounds = RectFromString(xAttribute.Value);
		}
		AddConfig(animationData, element);
		foreach (XElement item in element.Element("children").Elements())
		{
			Dictionary<string, string> config = GetConfig(item);
			if (config != null)
			{
				animationData.AddInstanceConfig(item.Name.ToString(), config);
			}
		}
		foreach (XElement item2 in element.Element("frames").Elements())
		{
			animationData.Add(GetAnimationFrame(item2));
		}
		return animationData;
	}

	private List<AnimationFrameData> GetAnimationFrame(XElement element)
	{
		List<AnimationFrameData> list = new List<AnimationFrameData>();
		foreach (XElement item2 in element.Elements())
		{
			AnimationFrameData animationFrameData = new AnimationFrameData();
			animationFrameData.Id = item2.Name.ToString();
			animationFrameData.Alpha = AttributToFloat(item2.Attribute("alpha"), 1f);
			animationFrameData.Position = VectorFromString((string?)item2.Attribute("position"), Vector2.Zero);
			animationFrameData.Rotation = AttributToFloat(item2.Attribute("rotation"), 0f);
			animationFrameData.Scale = VectorFromString((string?)item2.Attribute("scale"), Vector2.One);
			animationFrameData.Color = AttributToInt(item2.Attribute("color"), 16777215).ToRGBColor();
			animationFrameData.ColorRatio = AttributToFloat(item2.Attribute("colorRatio"), 0f);
			animationFrameData.Visible = AttributeToBool(item2.Attribute("visible"), defaultValue: true);
			AnimationFrameData item = animationFrameData;
			list.Add(item);
		}
		return list;
	}
}
