using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGetIPhoneButton : Sprite, IFreeable, IId
{
    public const string ID = "menu/McGetIPhoneButton";

    public string Id => "menu/McGetIPhoneButton";

    public static McGetIPhoneButton New()
    {
        McGetIPhoneButton mcGetIPhoneButton = StaticPool.New<McGetIPhoneButton>();
        mcGetIPhoneButton.RefreshProperties();
        return mcGetIPhoneButton;
    }

    public McGetIPhoneButton()
        : base("menu/McGetIPhoneButton")
    {
    }

    public void Free()
    {
        StaticPool.Free<McGetIPhoneButton>(this);
    }
}
