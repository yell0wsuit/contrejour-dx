namespace ContreJour.Gameplay
{
    public class FakeProcessor(string type, LevelBuilderBase builder) : TypeProcessorBase(type, builder)
    {
        private static readonly int StaticResult = 1;

        public override object ProcessItem(Hashtable item)
        {
            return StaticResult;
        }
    }
}
