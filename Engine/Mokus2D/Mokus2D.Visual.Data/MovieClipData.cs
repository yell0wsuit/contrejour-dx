using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data;

public class MovieClipData(string id) : TextureNodeData(id), IMovieClipData, ITextureNodeData, IConfig
{
    private readonly List<FrameData> _frames = [];

    public Vector2 Anchor { get; set; }

    public Vector2 Size { get; set; }

    public List<FrameData> Frames => _frames;
}
