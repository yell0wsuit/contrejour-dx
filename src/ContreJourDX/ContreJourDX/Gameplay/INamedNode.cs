using System.Numerics;

namespace ContreJourDX.Gameplay
{
    public interface INamedNode
    {
        string Name();

        Hashtable Config();

        Vector2 Size();
    }
}
