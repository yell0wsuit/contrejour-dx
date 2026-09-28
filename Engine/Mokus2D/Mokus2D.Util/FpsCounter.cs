using Mokus2D.Interfaces;

namespace Mokus2D.Util;

public class FpsCounter : IUpdatable
{
    protected readonly int FramesToCalculate;

    private int currentFrame;

    private float seconds;

    private float fps;

    public float Fps => fps;

    public FpsCounter(int framesToCalculate)
    {
        FramesToCalculate = framesToCalculate;
    }

    public void Update(float time)
    {
        currentFrame++;
        IncreaseFrameTime(time);
        if (currentFrame == FramesToCalculate)
        {
            CalculateFps();
        }
    }

    protected virtual void CalculateFps()
    {
        currentFrame = 0;
        fps = (float)FramesToCalculate / seconds;
        seconds = 0f;
    }

    protected virtual void IncreaseFrameTime(float time)
    {
        seconds += time;
    }
}
