using System.Numerics;

namespace ContreJourDX.Gameplay
{
    public class MenuPortal(Vector2 position) : Portal(null, position)
    {
        public override void Update(float time)
        {
            base.Update(time);
            foreach (Satellite part in Parts)
            {
                part.Update(time);
            }
        }
    }
}
