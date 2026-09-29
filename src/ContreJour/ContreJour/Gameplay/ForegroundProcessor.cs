namespace ContreJour.Gameplay
{
    public class ForegroundProcessor(LevelBuilderBase builder) : TypeProcessorBase("foreground", builder)
    {
        private static readonly int StaticResult = 1;

        public override object ProcessItem(Hashtable item)
        {
            Hashtable hashtable = item.GetHashtable("config");
            if (hashtable.NotExists("z"))
            {
                hashtable["z"] = "12";
            }
            if (hashtable.NotExists("clipType"))
            {
                hashtable["skipClip"] = "true";
            }
            return StaticResult;
        }
    }
}
