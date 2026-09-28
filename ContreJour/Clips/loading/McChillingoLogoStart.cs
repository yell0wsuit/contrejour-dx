using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChillingoLogoStart : MovieClip, IFreeable, IId
{
    public const string ID = "loading/McChillingoLogoStart";

    public string Id => "loading/McChillingoLogoStart";

    public static McChillingoLogoStart New()
    {
        McChillingoLogoStart mcChillingoLogoStart = StaticPool.New<McChillingoLogoStart>();
        mcChillingoLogoStart.RefreshProperties();
        return mcChillingoLogoStart;
    }

    public McChillingoLogoStart()
        : base("loading/McChillingoLogoStart")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChillingoLogoStart>(this);
    }
}
