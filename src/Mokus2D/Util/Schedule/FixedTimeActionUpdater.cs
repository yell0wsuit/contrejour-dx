using System;

namespace Mokus2D.Util.Schedule
{
    public class FixedTimeActionUpdater(float stepTime, Action<float> updateAction = null) : Updater
    {
        private readonly float _stepTime = stepTime;

        private float _currentTime;

        private readonly Action<float> _updateAction = updateAction;

        public override void Update(float time)
        {
            _currentTime += time;
            while (_currentTime >= _stepTime)
            {
                base.Update(_stepTime);
                _updateAction?.Invoke(_stepTime);
                _currentTime -= _stepTime;
            }
        }

        public void Reset()
        {
            _currentTime = 0f;
        }
    }
}
