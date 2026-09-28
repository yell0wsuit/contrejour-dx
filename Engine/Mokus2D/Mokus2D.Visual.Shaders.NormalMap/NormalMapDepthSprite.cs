using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Shaders.NormalMap;

public class NormalMapDepthSprite : Sprite<VertexPositionColorTexture3>
{
	private readonly ISpriteData _normalMapData;

	private readonly ISpriteData _depthData;

	public NormalMapDepthSprite(string name)
		: this(name, name + "NormalMap", name + "Depth")
	{
	}

	public NormalMapDepthSprite(string name, string normalMapName, string depthName)
		: base(name)
	{
		_normalMapData = Mokus2DGame.LoadSpriteData(normalMapName);
		_depthData = Mokus2DGame.LoadSpriteData(depthName);
		Quad<VertexPositionColorTexture3> quad = (Quad<VertexPositionColorTexture3>)Quad;
		quad.LeftTop.NormalMapTextureCoordinate = _normalMapData.TextureRect.LeftTop() / base.Texture.Bounds.Size();
		quad.RightTop.NormalMapTextureCoordinate = _normalMapData.TextureRect.RightTop() / base.Texture.Bounds.Size();
		quad.LeftBottom.NormalMapTextureCoordinate = _normalMapData.TextureRect.LeftBottom() / base.Texture.Bounds.Size();
		quad.RightBottom.NormalMapTextureCoordinate = _normalMapData.TextureRect.RightBottom() / base.Texture.Bounds.Size();
		quad.LeftTop.DepthTextureCoordinate = _depthData.TextureRect.LeftTop() / base.Texture.Bounds.Size();
		quad.RightTop.DepthTextureCoordinate = _depthData.TextureRect.RightTop() / base.Texture.Bounds.Size();
		quad.LeftBottom.DepthTextureCoordinate = _depthData.TextureRect.LeftBottom() / base.Texture.Bounds.Size();
		quad.RightBottom.DepthTextureCoordinate = _depthData.TextureRect.RightBottom() / base.Texture.Bounds.Size();
	}

	protected override void RefreshQuad()
	{
		base.RefreshQuad();
		Quad<VertexPositionColorTexture3> quad = (Quad<VertexPositionColorTexture3>)Quad;
		quad.LeftTop.Rotation = RotationRadians;
		quad.RightBottom.Rotation = RotationRadians;
		quad.RightTop.Rotation = RotationRadians;
		quad.LeftBottom.Rotation = RotationRadians;
		Vector2 scale = ScaleVec.Signs();
		quad.LeftTop.Scale = scale;
		quad.RightBottom.Scale = scale;
		quad.RightTop.Scale = scale;
		quad.LeftBottom.Scale = scale;
	}
}
