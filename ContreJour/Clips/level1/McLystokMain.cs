using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLystokMain : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McLystokMain";

    public McLystokMainContent content { get; protected set; }

    public McTear tear { get; protected set; }

    public string Id => "level1/McLystokMain";

    public static McLystokMain New()
    {
        McLystokMain mcLystokMain = StaticPool<McLystokMain>.New();
        mcLystokMain.RefreshProperties();
        return mcLystokMain;
    }

    public McLystokMain()
        : base("level1/McLystokMain")
    {
        content = new McLystokMainContent();
        AddChild("content", content);
        tear = new McTear();
        AddChild("tear", tear);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McLystokMain>.Free(this);
    }
}
