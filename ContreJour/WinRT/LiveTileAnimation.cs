using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Text;

namespace ContreJour.WinRT;

public abstract class LiveTileAnimation : AnimationNode
{
    private Label _countLabel;

    public int Count
    {
        set => _countLabel.SetText(value);
    }

    protected LiveTileAnimation(string name)
        : base(name)
    {
    }

    protected LiveTileAnimation(AnimationData animationData)
        : base(animationData)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        _countLabel = (Label)GetChild("count");
    }
}
