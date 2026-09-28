using Mokus2D.Visual.Text;

namespace ContreJour.Utils;

public class ContreJourLabel : Label
{
    public ContreJourLabel()
        : this(28f)
    {
    }

    public ContreJourLabel(float fontSize)
        : base("SegoePrint", fontSize)
    {
    }
}
