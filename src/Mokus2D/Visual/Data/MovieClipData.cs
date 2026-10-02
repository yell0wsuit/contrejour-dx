using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data
{
    public class MovieClipData(string id) : TextureNodeData(id), IMovieClipData, ITextureNodeData, IConfig
    {
        public Vector2 Anchor { get; set; }

        public Vector2 Size { get; set; }

        public List<FrameData> Frames { get; } = [];
    }
}
