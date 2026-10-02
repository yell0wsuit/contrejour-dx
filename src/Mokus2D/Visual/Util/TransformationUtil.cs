namespace Mokus2D.Visual.Util
{
    public static class TransformationUtil
    {
        public static bool ShouldRefreshNode(Node node)
        {
            return node.Visible && node.OnScreenCount > 0;
        }
    }
}
