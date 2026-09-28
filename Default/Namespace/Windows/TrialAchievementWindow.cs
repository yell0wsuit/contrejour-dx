using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual;

namespace Default.Namespace.Windows;

public class TrialAchievementWindow : PopUpWindow
{
    public TrialAchievementWindow()
    {
        AddChild(new LayerColor(Color.Red, "menu/whitePixel"));
        Mokus2DGame.Instance.KeysController.AddBackKeyListener(OnBack, -1);
    }

    private void OnBack()
    {
        Mokus2DGame.Instance.KeysController.StopPropagation();
        Mokus2DGame.Instance.KeysController.RemoveBackKeyListener(OnBack);
        base.Open = false;
    }
}
