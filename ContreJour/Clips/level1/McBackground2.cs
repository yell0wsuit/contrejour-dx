using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground2 : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McBackground2";

    public McBackground2Back huj { get; protected set; }

    public McSunBackground instance5232006 { get; protected set; }

    public McSunLight instance5232014 { get; protected set; }

    public McBlackSkyBackground instance5232032 { get; protected set; }

    public McBackground1Front instance5232034 { get; protected set; }

    public McBackground0Front instance5232042 { get; protected set; }

    public string Id => "level1/McBackground2";

    public static McBackground2 New()
    {
        McBackground2 mcBackground = StaticPool<McBackground2>.New();
        mcBackground.RefreshProperties();
        return mcBackground;
    }

    public McBackground2()
        : base("level1/McBackground2")
    {
        huj = new McBackground2Back();
        AddChild("huj", huj);
        instance5232006 = new McSunBackground();
        AddChild("instance5232006", instance5232006);
        instance5232014 = new McSunLight();
        AddChild("instance5232014", instance5232014);
        instance5232032 = new McBlackSkyBackground();
        AddChild("instance5232032", instance5232032);
        instance5232034 = new McBackground1Front();
        AddChild("instance5232034", instance5232034);
        instance5232042 = new McBackground0Front();
        AddChild("instance5232042", instance5232042);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McBackground2>.Free(this);
    }
}
