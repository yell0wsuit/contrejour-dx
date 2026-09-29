namespace ContreJour.Gameplay
{
    public class RopeMetrics(int parts, float partSize)
    {
        public int Parts { get; set; } = parts;

        public float PartSize { get; set; } = partSize;
    }
}
