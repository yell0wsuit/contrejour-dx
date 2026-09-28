using System;

namespace Mokus2D.Util.Schedule;

public class FixedStepsUpdater
{
    private readonly int _periodSteps;

    private int _updateStep;

    private readonly Action _updateAction;

    private int _currentStep;

    public int UpdateStep
    {
        get => _updateStep;
        set => _updateStep = value % _periodSteps;
    }

    public FixedStepsUpdater(Action updateAction, int periodSteps, int updateStep = 0)
    {
        _updateAction = updateAction;
        _periodSteps = periodSteps;
        UpdateStep = updateStep;
    }

    public void Update()
    {
        if (_currentStep % _periodSteps == _updateStep)
        {
            _updateAction();
        }
        _currentStep++;
    }

    public void Reset()
    {
        _currentStep = 0;
    }
}
