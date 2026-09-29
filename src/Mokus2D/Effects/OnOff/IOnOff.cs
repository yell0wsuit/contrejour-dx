namespace Mokus2D.Effects.OnOff;

public interface IOnOff
{
    bool IsOn { get; set; }

    void SetOn(bool value);
}
