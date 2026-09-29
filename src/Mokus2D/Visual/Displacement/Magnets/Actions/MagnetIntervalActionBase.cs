using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets.Actions
{
    public abstract class MagnetIntervalActionBase(GridMagnetBase gridMagnet, float timeout) : MagnetAction(gridMagnet)
    {
        private readonly float _timeout = timeout;

        private float _elapsed;

        public override bool Finished => _elapsed >= _timeout;

        protected abstract void UpdateMagnet(float ratio);

        public override void Update(float time)
        {
            base.Update(time);
            _elapsed += time;
            float ratio = (_elapsed / _timeout).Clamp(0f, 1f);
            UpdateMagnet(ratio);
        }
    }
}
