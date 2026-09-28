using Mokus2D.Effects.OnOff;
using Mokus2D.Visual;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.UI.Controls.Toggle;

public class ToggleButtonBase : MovieClip
{
    public ToggleButton Button { get; private set; }

    public ToggleButtonBase(string name)
        : base(name)
    {
    }

    public ToggleButtonBase(IMovieClipData data)
        : base(data)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        Button = CreateButton();
    }

    protected virtual ToggleButton CreateButton()
    {
        return new ToggleButton(this, new FrameOnOff(this));
    }
}
