using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLock : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McLock";

    public string Id => "menu2/McLock";

    public static McLock New()
    {
        McLock mcLock = StaticPool<McLock>.New();
        mcLock.RefreshProperties();
        return mcLock;
    }

    public McLock()
        : base("menu2/McLock")
    {
    }

    public void Free()
    {
        StaticPool<McLock>.Free(this);
    }
}
