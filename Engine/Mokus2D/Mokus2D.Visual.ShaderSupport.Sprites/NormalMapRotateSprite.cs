using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.ShaderSupport.Sprites;

public class NormalMapRotateSprite : Sprite<VertexNormalMapRotate>
{
	public bool SkipSelfTransform;

	public NormalMapRotateSprite(string name)
		: base(name)
	{
	}

	protected override void RefreshQuad()
	{
		if (!SkipSelfTransform)
		{
			base.RefreshQuad();
		}
		Quad<VertexNormalMapRotate> quad = (Quad<VertexNormalMapRotate>)Quad;
		quad.LeftTop.Scale = ScaleVec;
		quad.RightTop.Scale = ScaleVec;
		quad.RightBottom.Scale = ScaleVec;
		quad.LeftBottom.Scale = ScaleVec;
		float rotationRadians = RotationRadians;
		quad.LeftTop.Rotation = rotationRadians;
		quad.RightTop.Rotation = rotationRadians;
		quad.RightBottom.Rotation = rotationRadians;
		quad.LeftBottom.Rotation = rotationRadians;
	}
}
