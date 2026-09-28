// Mokus2D sprite shader, recovered from the Windows 8 package (SpriteShader.dx.fx).
// The original #include "Maths.fx" was not shipped and nothing from it is used.
// Build for DesktopGL with: mgcb /platform:DesktopGL ... -> SpriteShader.ogl.mgfxo

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture Texture;
sampler TextureSampler = sampler_state { texture = <Texture> ; magfilter = Anisotropic; minfilter = Anisotropic; mipfilter=LINEAR; AddressU = Clamp; AddressV = Clamp;};

float4x4 Matrix;
bool TintEnabled = true;

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoords : TEXCOORD0;
	float TintRatio : BLENDWEIGHT0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoords : TEXCOORD0;
	float TintRatio : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, Matrix);
	output.Color = input.Color;
	output.TextureCoords = input.TextureCoords;
	output.TintRatio = input.TintRatio;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(TextureSampler, input.TextureCoords);
	float alpha = result.a;
	if (TintEnabled)
	{
		result = lerp(result, input.Color*alpha, input.TintRatio);
		result.a = alpha;
		result *= input.Color.a;
	}
	else
	{
		//premultiplied
		result *= input.Color * input.Color.a;
		result.a = alpha * input.Color.a;
	}
	return result;
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
