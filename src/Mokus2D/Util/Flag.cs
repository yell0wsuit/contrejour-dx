namespace Mokus2D.Util
{
    public class Flag(bool on = true)
    {
        private bool _on = on;

        public bool Use()
        {
            if (_on)
            {
                _on = false;
                return true;
            }
            return false;
        }

        public void SetOn()
        {
            _on = true;
        }
    }
}
