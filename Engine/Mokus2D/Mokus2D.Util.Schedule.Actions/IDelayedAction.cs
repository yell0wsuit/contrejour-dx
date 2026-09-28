namespace Mokus2D.Util.Schedule.Actions;

public interface IDelayedAction
{
    void Execute();

    void Schedule();
}
