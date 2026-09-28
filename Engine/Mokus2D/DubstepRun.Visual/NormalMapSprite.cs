using Microsoft.Xna.Framework;
using Mokus2D;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Util;

namespace DubstepRun.Visual;

public class NormalMapSprite : Sprite<VertexPositionColorTexture2>
{
	private readonly ISpriteData _normalMapData;

	public float Z;

	public NormalMapSprite(string name, string normalMapName)
		: base(name)
	{
		_normalMapData = Mokus2DGame.LoadSpriteData(normalMapName);
		Quad<VertexPositionColorTexture2> quad = (Quad<VertexPositionColorTexture2>)Quad;
		quad.LeftTop.NormalMapTextureCoordinate = _normalMapData.TextureRect.LeftTop() / base.Texture.Bounds.Size();
		quad.RightTop.NormalMapTextureCoordinate = _normalMapData.TextureRect.RightTop() / base.Texture.Bounds.Size();
		quad.LeftBottom.NormalMapTextureCoordinate = _normalMapData.TextureRect.LeftBottom() / base.Texture.Bounds.Size();
		quad.RightBottom.NormalMapTextureCoordinate = _normalMapData.TextureRect.RightBottom() / base.Texture.Bounds.Size();
	}

	protected override void RefreshQuad()
	{
		base.RefreshQuad();
		Quad<VertexPositionColorTexture2> quad = (Quad<VertexPositionColorTexture2>)Quad;
		float rootRotationRadians = this.GetRootRotationRadians();
		Vector2 scale = ScaleVec.Signs();
		RefreshQuadVertex(ref quad.LeftTop, rootRotationRadians, scale);
		RefreshQuadVertex(ref quad.RightBottom, rootRotationRadians, scale);
		RefreshQuadVertex(ref quad.RightTop, rootRotationRadians, scale);
		RefreshQuadVertex(ref quad.LeftBottom, rootRotationRadians, scale);
	}

	private void RefreshQuadVertex(ref VertexPositionColorTexture2 vertex, float rotation, Vector2 scale)
	{
		vertex.Rotation = rotation;
		vertex.Scale = scale;
		vertex.Position.Z = Z;
	}
}
