using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace Default.Namespace;

public class AccelerometerNode : Node
{
    private const float StepCoeff = 0.015f;

    private const float MinChange = 0.05f;

    private const float SpeedStep = 0.05f;

    protected Vector2 accelerometerOffset;

    protected bool accelerometerUsed;

    protected Vector2 maxAccOffset = new Vector2((float)Math.PI / 22f, 80f);

    private Vector2 speed = Vector2.Zero;

    private readonly Accelerometer accelerometer = Accelerometer.GetDefault();

    private Vector2 _accelerometerValue;

    private Vector2 Acceleration
    {
        get
        {
            if (accelerometer == null)
            {
                return Vector2.Zero;
            }
            return _accelerometerValue;
        }
    }

    public unsafe AccelerometerNode()
    {
        if (accelerometer != null)
        {
            Accelerometer val = accelerometer;
            WindowsRuntimeMarshal.AddEventHandler(new Func<TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>, EventRegistrationToken>(val, (nint)__ldftn(Accelerometer.add_ReadingChanged)), new Action<EventRegistrationToken>(val, (nint)__ldftn(Accelerometer.remove_ReadingChanged)), OnReadingChanged);
        }
    }

    private void UpdateOffset(Vector2 acceleration)
    {
        Vector2 vector = default(Vector2);
        vector.X = Maths.Clamp((0f - acceleration.Y) * 3f * maxAccOffset.X, 0f - maxAccOffset.X, maxAccOffset.X);
        vector.Y = Maths.Clamp((acceleration.X + 0.7f) * maxAccOffset.Y * 2f, 0f - maxAccOffset.Y, maxAccOffset.Y);
        if (!accelerometerUsed)
        {
            accelerometerOffset = vector;
            accelerometerUsed = true;
        }
        else
        {
            accelerometerOffset = VectorExtensions.StepTo(step: ((vector - accelerometerOffset) * 0.015f).Abs(), source: accelerometerOffset, target: vector);
        }
    }

    public override void Update(float time)
    {
        Vector2 acceleration = Acceleration;
        if (acceleration.X > 0f)
        {
            acceleration.X *= -1f;
        }
        UpdateOffset(acceleration);
    }

    private void OnReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
    {
        _accelerometerValue = new Vector2((float)args.Reading.AccelerationY, (float)args.Reading.AccelerationX);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
