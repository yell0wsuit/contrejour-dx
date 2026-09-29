using Microsoft.Xna.Framework;

namespace Default.Namespace;

public interface INamedNode
{
    string Name();

    Hashtable Config();

    Vector2 Size();
}
