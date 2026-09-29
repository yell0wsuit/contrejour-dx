namespace Default.Namespace;

public interface ISpikesDestroyable : IBodyClip
{
    void Explode();

    void DoExplode();

    bool CanDie();
}
