using System.Xml.Linq;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Content.Serialization;

public class MovieClipDeserializer(IGraphicsLoader loader) : NodeDeserializerBase<IMovieClipData>(loader)
{
    public override IMovieClipData Deserialize(string id, XElement element)
    {
        MovieClipData movieClipData = new(id);
        AddConfigAndScaleFactor(movieClipData, element);
        SetTexture(element, movieClipData);
        foreach (XElement item in element.Descendants("frame"))
        {
            movieClipData.Frames.Add(GetFrame(item));
        }
        movieClipData.Anchor = AnchorFromXml(element);
        movieClipData.Size = VectorFromString((string)element.Attribute("size"));
        return movieClipData;
    }
}
