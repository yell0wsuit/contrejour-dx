using Mokus2D.Visual.Text;

namespace ContreJour.Utils
{
    public class ContreJourLabel(float fontSize) : Label("SegoePrint", fontSize)
    {
        public ContreJourLabel()
            : this(28f)
        {
        }
    }
}
