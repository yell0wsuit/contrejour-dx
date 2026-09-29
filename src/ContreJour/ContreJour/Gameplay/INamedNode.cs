using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay
{
    public interface INamedNode
    {
        string Name();

        Hashtable Config();

        Vector2 Size();
    }
}
