using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.ShaderSupport.Parameters;

namespace Mokus2D.Visual.Shaders.NormalMap;

public abstract class NormalMapEffectBase : TextureMatrixEffectBase
{
    public readonly ShaderParameterInt LightsCount;

    public readonly ShaderParameterFloat AmbientLightPower;

    public readonly ShaderParameterVector3 AmbientLightVector;

    public readonly ShaderParameterColor AmbientLightColor;

    public readonly ShaderParameterFloat MaxLightPower;

    public readonly ShaderParameterFloat DiffuseLightPower;

    private readonly EffectParameter _lightPosition;

    private readonly EffectParameter _lightPower;

    private readonly EffectParameter _lightColor;

    private readonly EffectParameter _lightDistanceRate;
    private bool _transformationDirty;

    private readonly Vector3[] _lightPositionValuesTransformed;

    private readonly Vector3[] _lightPositionValues;

    private bool _lightPositionDirty = true;

    private readonly float[] _lightPowerValues;

    private bool _lightPowerDirty = true;

    private readonly Vector4[] _lightColorValues;

    private bool _lightColorDirty = true;

    private readonly float[] _lightDistanceRateValues;

    private bool _lightDistanceRateDirty = true;

    private readonly LightData[] _lightsData;

    public int MaxLightsCount { get; private set; }

    public LightData this[int index] => _lightsData[index];

    public Node TransformationNode
    {
        get;
        set
        {
            if (field != value)
            {
                field?.TransformationsRefreshedEvent -= OnTransformationsRefreshed;
                field = value;
                field?.TransformationsRefreshedEvent += OnTransformationsRefreshed;
                _transformationDirty = true;
            }
        }
    }

    protected NormalMapEffectBase(string path, int maxLightsCount)
        : base(path)
    {
        MaxLightsCount = maxLightsCount;
        _lightsData = new LightData[maxLightsCount];
        _lightPositionValuesTransformed = new Vector3[MaxLightsCount];
        _lightPositionValues = new Vector3[MaxLightsCount];
        _lightPowerValues = new float[MaxLightsCount];
        _lightDistanceRateValues = new float[MaxLightsCount];
        _lightColorValues = new Vector4[MaxLightsCount];
        _lightPosition = Parameters["LightPosition"];
        _lightPower = Parameters["LightPower"];
        _lightColor = Parameters["LightColor"];
        _lightDistanceRate = Parameters["LightDistanceRate"];
        AmbientLightPower = new ShaderParameterFloat(Parameters, "AmbientLightPower");
        AmbientLightVector = new ShaderParameterVector3(Parameters, "AmbientLightVector");
        AmbientLightColor = new ShaderParameterColor(Parameters, "AmbientLightColor");
        MaxLightPower = new ShaderParameterFloat(Parameters, "MaxLightPower");
        DiffuseLightPower = new ShaderParameterFloat(Parameters, "DiffuseLightPower");
        _lightsData[0] = new LightData(this, 0, new Vector3(640f, 360f, 100f), 1f, Color.Red);
        for (int i = 1; i < maxLightsCount; i++)
        {
            _lightsData[i] = new LightData(this, i, new Vector3(0f), 0f);
        }
        LightsCount = new ShaderParameterInt(Parameters, "LightsCount");
    }

    public override void Apply(Matrix matrix, Texture2D texture)
    {
        if (_transformationDirty || _lightPositionDirty)
        {
            _transformationDirty = false;
            _lightPositionDirty = false;
            RefreshTransformations();
        }
        if (_lightPowerDirty)
        {
            _lightPowerDirty = false;
            _lightPower.SetValue(_lightPowerValues);
        }
        if (_lightColorDirty)
        {
            _lightColorDirty = false;
            _lightColor.SetValue(_lightColorValues);
        }
        if (_lightDistanceRateDirty)
        {
            _lightDistanceRateDirty = false;
            _lightDistanceRate.SetValue(_lightDistanceRateValues);
        }
        base.Apply(matrix, texture);
    }

    private void RefreshTransformations()
    {
        for (int i = 0; i < MaxLightsCount; i++)
        {
            if (TransformationNode != null)
            {
                ref Vector3 reference = ref _lightPositionValuesTransformed[i];
                reference = Vector3.Transform(_lightPositionValues[i], TransformationNode.CompositeState.Matrix);
            }
            else
            {
                ref Vector3 reference2 = ref _lightPositionValuesTransformed[i];
                reference2 = _lightPositionValues[i];
            }
        }
        _lightPosition.SetValue(_lightPositionValuesTransformed);
    }

    internal void SetLightPosition(int index, Vector3 value)
    {
        _lightPositionValues[index] = value;
        _lightPositionDirty = true;
    }

    internal void SetLightPower(int index, float value)
    {
        _lightPowerValues[index] = value;
        _lightPowerDirty = true;
    }

    internal void SetLightColor(int index, Color value)
    {
        ref Vector4 reference = ref _lightColorValues[index];
        reference = value.ToVector4();
        _lightColorDirty = true;
    }

    internal void SetLightDistanceRate(int index, float value)
    {
        _lightDistanceRateValues[index] = value;
        _lightDistanceRateDirty = true;
    }

    private void OnTransformationsRefreshed()
    {
        _transformationDirty = true;
    }
}
