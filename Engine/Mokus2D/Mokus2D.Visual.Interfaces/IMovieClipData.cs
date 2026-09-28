using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Interfaces;

public interface IMovieClipData : ITextureNodeData, IConfig
{
	Vector2 Anchor { get; }

	Vector2 Size { get; }

	List<FrameData> Frames { get; }
}
