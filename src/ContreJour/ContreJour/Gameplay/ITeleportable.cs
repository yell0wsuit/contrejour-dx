namespace ContreJour.Gameplay
{
    public interface ITeleportable
    {
        bool SnotEnabled { set; }

        void Teleport(BodyClip teleport);

        void SetScaleTime(float scale, float time);

        void AfterTeleport();

        void ForceClipPosition();

        bool CanTeleport();
    }
}
