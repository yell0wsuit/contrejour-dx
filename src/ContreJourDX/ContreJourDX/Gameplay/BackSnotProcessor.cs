namespace ContreJourDX.Gameplay
{
    public class BackSnotProcessor(LevelBuilderBase builder) : SnotProcessor(builder, "backSnot", 100f * builder.EngineConfig.SizeMultiplier)
    {
        public override float GetDensityTotal(int index, int total)
        {
            return 0.3f;
        }
    }
}
